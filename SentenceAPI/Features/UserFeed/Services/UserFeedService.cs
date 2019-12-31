using MongoDB.Driver;

using SentenceAPI.Features.Authentication.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Exceptions;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using MongoDB.Bson;
using SharedLibrary.Loggers.Configuration;

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
        private readonly IUserFriendsService userFriendsService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        private readonly IUserService<UserInfo> userService;
        private readonly IUserPhotoService userPhotoService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
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
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);
        }

        public async Task<UsersFeedDto> GetUserFeedAsync(long userID)
        {
            try
            {
                List<long> subscriptionsID = (await userFriendsService.GetSubscriptionsAsync(userID))
                    .Select(uf => uf.UserID).ToList();
                subscriptionsID.Add(userID);

                Dictionary<long, ObjectId> usersPhotoes = await GetUsersPhotoesAsync(subscriptionsID).ConfigureAwait(false);
                Dictionary<long, string> usersRawPhotoes = await GetUsersRawPhotoesAsync(usersPhotoes).ConfigureAwait(false);

                await database.Connect().ConfigureAwait(false);
                var usersFeed = await database.GetCombined(new InFilter<long>("userID", subscriptionsID), "userID",
                    (typeof(UserInfo), "_id", new[] { "name", "surname" })).ConfigureAwait(false);

                return new UsersFeedDto(usersFeed.ToList(), usersRawPhotoes);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        private async Task<Dictionary<long, string>> GetUsersRawPhotoesAsync(Dictionary<long, ObjectId> userPhotoes)
        {
            Dictionary<long, string> userRawPhotoes = new Dictionary<long, string>();

            foreach (var (userID, photoID) in userPhotoes)
            {
                byte[] currentPhoto = await userPhotoService.GetRawPhotoAsync(photoID).ConfigureAwait(false);
                
                userRawPhotoes.Add(userID, Convert.ToBase64String(currentPhoto));
            }

            return userRawPhotoes;
        }

        private async Task<Dictionary<long, ObjectId>> GetUsersPhotoesAsync(IEnumerable<long> userIDs)
        {
            Dictionary<long, ObjectId> userPhotoes = new Dictionary<long, ObjectId>();

            foreach (long userID in userIDs)
            {
                ObjectId id = (await userPhotoService.GetPhotoAsync(userID).ConfigureAwait(false)).CurrentPhotoID;
                userPhotoes.Add(userID, id);
            }

            return userPhotoes;
        }

        public async Task<UsersFeedDto> GetUserFeedAsync(string token)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                return await GetUserFeedAsync(userID);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        public async Task InsertUserPostAsync(Models.UserFeed userFeed)
        {
            try
            {
                await database.Connect();
                await database.Insert(userFeed);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while inserting new post");
            }
        }

        public async Task InsertUserPostAsync(string token, string message)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));
                await InsertUserPostAsync(new Models.UserFeed()
                {
                    UserID = userID,
                    Message = message,
                    PublicationDate = DateTime.Now,
                });
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                 exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while inserting new post");
            }
        }
    }
}
