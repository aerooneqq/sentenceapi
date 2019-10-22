using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using Microsoft.Extensions.Caching.Memory;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using SentenceAPI.Features.UserPhoto.Models;
using SharedLibrary.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
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

        public async Task CreateUserPhoto(long userID)
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

                await database.Insert(new Models.UserPhoto(userID, string.Empty)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("The error occured while creating photo-record");
            }
        }

        public async Task<Models.UserPhoto> GetPhoto(long userID)
        {
            try
            {
                if (cacheService.Contains(GetUserPhotoCacheKey(userID)))
                {
                    return new Models.UserPhoto(userID, (string)cacheService.GetValue(GetUserPhotoCacheKey(userID)));
                }

                await database.Connect().ConfigureAwait(false);

                var filter = new EqualityFilter<long>(typeof(Models.UserPhoto).GetBsonPropertyName("UserID"), userID);
                var userPhoto = (await database.Get(filter).ConfigureAwait(false)).FirstOrDefault();

                cacheService.TryInsert(GetUserPhotoCacheKey(userID), userPhoto.Photo);

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

        public async Task<Models.UserPhoto> GetPhoto(string token)
        {
            try
            {
                return await GetPhoto(long.Parse(tokenService.GetTokenClaim(token, "ID")))
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("The error occured while getting the photo");
            }
        }

        public async Task UpdatePhoto(Models.UserPhoto userPhoto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var oldUserPhoto = (await database.Get(new EqualityFilter<long>(
                    typeof(UserPhoto.Models.UserPhoto).GetBsonPropertyName("UserID"), userPhoto.UserID)).
                    ConfigureAwait(false)).FirstOrDefault();

                if (oldUserPhoto == null)
                {
                    await database.Insert(userPhoto);
                    return;
                }

                oldUserPhoto.Photo = userPhoto.Photo;

                await database.Update(oldUserPhoto).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                throw new DatabaseException("Error occured while updating the photo");
            }
        }
    }
}
