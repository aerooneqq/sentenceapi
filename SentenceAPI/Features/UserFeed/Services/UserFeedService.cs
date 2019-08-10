using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Databases.MongoDB.Interfaces;
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
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.Databases.Filters;

namespace SentenceAPI.Features.UserFeed.Services
{
    public class UserFeedService : IUserFeedService
    {
        #region Services
        private IUserFriendsService userFriendsService;
        private IDatabaseService<Models.UserFeed> mongoDBService;
        private ILogger<ApplicationError> exceptionLogger;
        private ITokenService tokenService;
        #endregion

        #region Factories
        IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ITokenServiceFactory tokenServiceFactory;
        private IUserFriendsServiceFactory userFriendsServiceFactory;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<Models.UserFeed> mongoDBServiceBuilder;
        #endregion

        public UserFeedService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory as IMongoDBServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;
            userFriendsServiceFactory = factoriesManager[typeof(IUserFriendsServiceFactory)].Factory as IUserFriendsServiceFactory;
            tokenServiceFactory = factoriesManager[typeof(ITokenServiceFactory)].Factory as ITokenServiceFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService<Models.UserFeed>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").SetConnectionString()
                .SetDatabaseName("SentenceDatabase").SetCollectionName().Build();

            exceptionLogger = loggerFactory.GetExceptionLogger();
            userFriendsService = userFriendsServiceFactory.GetSerivce();
            tokenService = tokenServiceFactory.GetService();
        }


        public async Task<IEnumerable<Models.UserFeed>> GetUserFeed(long userID)
        {
            try
            {
                List<long> subscriptionsID = userFriendsService.GetSubscriptions(userID).GetAwaiter().GetResult()
                    .Select(uf => uf.UserID).ToList();
                subscriptionsID.Add(userID);

                await mongoDBService.Connect();
                return mongoDBService.Get(new InFilter<long>("userID", subscriptionsID))
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
                await mongoDBService.Connect();
                await mongoDBService.Insert(userFeed);
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
