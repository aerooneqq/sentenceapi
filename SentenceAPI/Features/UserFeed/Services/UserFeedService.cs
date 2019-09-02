using MongoDB.Driver;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.Features.UserFriends.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Exceptions;

namespace SentenceAPI.Features.UserFeed.Services
{
    public class UserFeedService : IUserFeedService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<Models.UserFeed> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private IUserFriendsService userFriendsService;
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion

        #region Factories
        IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public UserFeedService()
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserFeed>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        public async Task<IEnumerable<Models.UserFeed>> GetUserFeed(long userID)
        {
            try
            {
                List<long> subscriptionsID = (await userFriendsService.GetSubscriptions(userID))
                    .Select(uf => uf.UserID).ToList();
                subscriptionsID.Add(userID);

                await database.Connect();
                return database.Get(new InFilter<long>("userID", subscriptionsID))
                    .GetAwaiter().GetResult().OrderBy(uf => uf.PublicationDate);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        public async Task<IEnumerable<Models.UserFeed>> GetUserFeed(string token)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                return await GetUserFeed(userID);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        public async Task InsertUserPost(Models.UserFeed userFeed)
        {
            try
            {
                await database.Connect();
                await database.Insert(userFeed);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while inserting new post");
            }
        }

        public async Task InsertUserPost(string token, string message)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));
                await InsertUserPost(new Models.UserFeed()
                {
                    UserID = userID,
                    Message = message,
                    PublicationDate = DateTime.Now,
                });
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while inserting new post");
            }
        }
    }
}
