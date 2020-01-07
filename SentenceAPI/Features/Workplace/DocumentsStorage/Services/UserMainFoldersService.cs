using System;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using MongoDB.Bson;

using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Workplace.DocumentsStorage.Exceptions;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Services
{
    public class UserMainFoldersService : IUserMainFoldersService
    {
        private static readonly string databaseConfigFile = "mongo_database_config.json";

        #region Database
        private readonly IDatabaseService<UserMainFolders> database;
        private readonly IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFolderService folderService;
        #endregion


        public UserMainFoldersService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);

            databaseManager.MongoDBFactory.GetDatabase<UserMainFolders>().TryGetTarget(out database);
            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }

        public async Task<ObjectId> CreateNewUserMainFolders(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var userMainFolders = (await database.Get(new EqualityFilter<ObjectId>("userID", userID))
                    .ConfigureAwait(false)).FirstOrDefault();

                if (userMainFolders is {})
                {
                    throw new UserMainFolderAlreadyExistsException();
                }

                ObjectId projectFolderID = await folderService.CreateFolder(userID, ObjectId.Empty, "Projects").
                    ConfigureAwait(false);

                ObjectId localFolderID = await folderService.CreateFolder(userID, ObjectId.Empty, "Local").
                    ConfigureAwait(false);

                ObjectId userMainFoldersID = ObjectId.GenerateNewId();
                var userMainFolder = new UserMainFolders()
                {
                    ID = userMainFoldersID,
                    LocalFolderID = localFolderID,
                    ProjectsFolderID = projectFolderID,
                    UserID = userID
                };

                await database.Insert(userMainFolder);
                return userMainFoldersID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while creating default folders");
            }
        }

        public async Task<UserMainFolders> GetUserMainFoldersAsync(ObjectId userID)
        {
            try
            {
                return (await database.Get(new EqualityFilter<ObjectId>("userID", userID))
                    .ConfigureAwait(false)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while getting the main folders");
            }
        }

        public async Task UpdateUserMainFolders(UserMainFolders userMainFolders)
        {
            try
            {
                await database.Update(userMainFolders).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while updating the main folders");
            }
        }
    }
}