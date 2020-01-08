using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.MongoDB.Interfaces;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.Caching;
using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;

using MongoDB.Bson;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;

namespace SentenceAPI.Features.UserPhoto.Services
{
    public class UserPhotoService : IUserPhotoService
    {
        #region Static fields 
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static readonly string userPhotoCacheKey = "user_photo_";
        #endregion

        #region Database
        private readonly IDatabaseService<Models.UserPhoto> database;
        private readonly IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        private readonly ICacheService cacheService = CacheService.Service;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public UserPhotoService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserPhoto>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            
            logConfiguration = new LogConfiguration(this.GetType());
        }

        public async Task CreateUserPhotoAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<ObjectId>(typeof(Models.UserPhoto).GetBsonPropertyName("UserID"), userID);
                var userPhoto = await database.Get(filter).ConfigureAwait(false);

                if (userPhoto.ToList().Count != 0)
                {
                    throw new ArgumentException($"Attempt to create new photo with id, for which the" +
                        $" photo already exists. ID: {userID}");
                }

                await database.Insert(new Models.UserPhoto(userID)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating photo-record");
            }
        }

        public async Task<byte[]> GetRawPhotoAsync(ObjectId gridFSPhotoID)
        { 
            try
            {
                await database.Connect().ConfigureAwait(false);

                IMongoDBService<Models.UserPhoto> mongoDatabase = (IMongoDBService<Models.UserPhoto>)database;
                byte[] rawPhoto = await mongoDatabase.GridFS.GetFile(gridFSPhotoID).ConfigureAwait(false);

                return rawPhoto;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting raw photo");
            }
        }

        public async Task<Models.UserPhoto> GetPhotoAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<ObjectId>(typeof(Models.UserPhoto).GetBsonPropertyName("UserID"), userID);
                var userPhotoes = (await database.Get(filter).ConfigureAwait(false)).ToList();

                if (userPhotoes is null || userPhotoes.Count < 1)
                {
                    return null;
                }

                return userPhotoes[0];
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting your photo");
            }
        }

        private string GetUserPhotoCacheKey(ObjectId userID)
        {
            return userPhotoCacheKey + userID;
        }

        public async Task<Models.UserPhoto> GetPhotoAsync(string token)
        {
            try
            {
                return await GetPhotoAsync(ObjectId.Parse(tokenService.GetTokenClaim(token, "ID")))
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting the photo");
            }
        }

        public async Task InsertUserPhotoModel(Models.UserPhoto userPhoto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Insert(userPhoto).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating new user photo model");
            }
        }

        /// <summary>
        /// This method updates the user's photo. If the photo's hash already in the GridFSPhotoes,
        /// then we just need to return it's id, otherwise we upload the new file (user photo),
        /// and update the UserPhoto model.
        /// </summary>
        public async Task<ObjectId> UpdatePhotoAsync(Models.UserPhoto userPhoto, 
                                                     byte[] newPhoto,
                                                     string fileName)
        {
            try
            {
                string photoHash = newPhoto.GetMD5Hash();
                var (checkResult, existingPhotoID) = CheckIfHashInPhotoes(userPhoto, photoHash);

                if (checkResult)
                { 
                    return existingPhotoID;
                }

                IMongoDBService<Models.UserPhoto> mongoDatabase = (IMongoDBService<Models.UserPhoto>)database;

                await database.Connect().ConfigureAwait(false);
                ObjectId newPhotoID = await mongoDatabase.GridFS.AddFile(newPhoto, fileName).ConfigureAwait(false);

                userPhoto.GridFSPhotoes.Add(newPhotoID.ToString(), photoHash);
                userPhoto.CurrentPhotoID = newPhotoID;

                await database.Update(userPhoto).ConfigureAwait(false);

                return newPhotoID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while updating the photo");
            }
        }

        /// <summary>
        /// Checks if the wanted hash is in the dictionary of UserPhoto object (id - hash dictionary)
        /// </summary>
        private (bool result, ObjectId photoID) CheckIfHashInPhotoes(Models.UserPhoto userPhoto, string wantedHash)
        {
            foreach (var (id, hash) in userPhoto.GridFSPhotoes)
            {
                if (hash == wantedHash)
                {
                    return (true, ObjectId.Parse(id));
                }
            }

            return (false, ObjectId.Empty);
        }
    }
}
