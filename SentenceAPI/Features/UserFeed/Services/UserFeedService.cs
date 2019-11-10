using MongoDB.Driver;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

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
using DataAccessLayer.Aggregations.Interfaces;
using DataAccessLayer.Aggregations;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

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

        public async Task<UsersFeedDto> GetUserFeed(long userID)
        {
            try
            {
                List<long> subscriptionsID = (await userFriendsService.GetSubscriptions(userID))
                    .Select(uf => uf.UserID).ToList();
                subscriptionsID.Add(userID);

                Dictionary<long, string> usersPhotoes = await GetUsersPhotoes(subscriptionsID);

                await database.Connect().ConfigureAwait(false);
                var usersFeed = await database.GetCombined(new InFilter<long>("userID", subscriptionsID), "userID",
                    (typeof(UserInfo), "_id", new[] { "name", "surname" })).ConfigureAwait(false);

                return new UsersFeedDto(usersFeed.ToList(), usersPhotoes);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        private async Task<Dictionary<long, string>> GetUsersPhotoes(IEnumerable<long> userIDs)
        {
            Dictionary<long, string> userPhotoes = new Dictionary<long, string>();

            foreach (long userID in userIDs)
            {
                //userPhotoes.Add(userID, (await userPhotoService.GetPhotoAsync(userID).ConfigureAwait(false)).Photo);
            }

            return userPhotoes;
        }

        public async Task<UsersFeedDto> GetUserFeed(string token)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                return await GetUserFeed(userID);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                await exceptionLogger.Log(new ApplicationError(ex));
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
                await exceptionLogger.Log(new ApplicationError(ex));
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
                await exceptionLogger.Log(new ApplicationError(ex));
                throw new DatabaseException("Error occured while inserting new post");
            }
        }
    }
}
