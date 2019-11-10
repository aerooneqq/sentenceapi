using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.MongoDB.Interfaces;

using Microsoft.Extensions.Caching.Memory;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;
using SentenceAPI.Features.UserPhoto.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.Caching;
using SharedLibrary.FactoriesManager; 
using SharedLibrary.FactoriesManager.Interfaces;
using MongoDB.Bson;

namespace SentenceAPI.Features.UserPhoto.Services
{
    public class UserPhotoService : IUserPhotoService
    {
        #region Static fields 
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ServiceName = "UserPhotoService",
            ControllerName = string.Empty
        };
        private static readonly string userPhotoCacheKey = "user_photo_";
        #endregion

        #region Database
        private readonly IDatabaseService<Models.UserPhoto> database;
        private readonly IConfigurationBuilder configurationBuilder;
        private readonly DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        private readonly ICacheService cacheService = CacheService.Service;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public UserPhotoService()
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserPhoto>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                    .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;

            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        public async Task CreateUserPhotoAsync(long userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<long>(typeof(Models.UserPhoto).GetBsonPropertyName("UserID"), userID);
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
                await exceptionLogger.Log(new ApplicationError(ex));
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
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("The error occured while getting raw photo");
            }
        }

        public async Task<Models.UserPhoto> GetPhotoAsync(long userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<long>(typeof(Models.UserPhoto).GetBsonPropertyName("UserID"), userID);
                var userPhoto = (await database.Get(filter).ConfigureAwait(false)).FirstOrDefault();

                if (userPhoto is null) 
                {
                    return null;
                }

                return userPhoto;
            }
            catch (Exception ex) 
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("The error occured while getting your photo");
            }
        }

        private string GetUserPhotoCacheKey(long userID)
        {
            return userPhotoCacheKey + userID;
        }

        public async Task<Models.UserPhoto> GetPhotoAsync(string token)
        {
            try
            {
                return await GetPhotoAsync(long.Parse(tokenService.GetTokenClaim(token, "ID")))
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while creating new user photo model");
            }
        }

        public async Task<ObjectId> UpdatePhotoAsync(Models.UserPhoto userPhoto, 
                                                     byte[] newPhoto,
                                                     string fileName)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                IMongoDBService<Models.UserPhoto> mongoDatabase = (IMongoDBService<Models.UserPhoto>)database;
                
                //await mongoDatabase.GridFS.DeleteFile(userPhoto.PhotoGridFSId).ConfigureAwait(false);
                ObjectId newPhotoID = await mongoDatabase.GridFS.AddFile(newPhoto, fileName).ConfigureAwait(false);

                userPhoto.PhotoGridFSId = newPhotoID;
                await database.Update(userPhoto, new string[] {"PhotoGridFSId"});

                return newPhotoID;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("Error occured while updating the photo");
            }
        }
    }
}
