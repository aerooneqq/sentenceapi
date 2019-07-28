using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.UserFriends.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Services
{
    public class UserFriendsService : IUserFriendsService
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IMongoDBService<Models.UserFriends> mongoDBService;
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ITokenServiceFactory tokenServiceFactory;
        private IUserServiceFactory userServiceFactory;
        private ILoggerFactory loggerFactory;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<Models.UserFriends> mongoDBServiceBuilder;
        #endregion

        public UserFriendsService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory
                as IMongoDBServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory
                as ILoggerFactory;
            userServiceFactory = factoriesManager[typeof(IUserServiceFactory)].Factory
                as IUserServiceFactory;
            tokenServiceFactory = factoriesManager[typeof(ITokenServiceFactory)].Factory
                as ITokenServiceFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService
                <Models.UserFriends>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").SetConnectionString()
                .SetDatabaseName("SentenceDatabase").SetCollectionName().Build();

            exceptionLogger = loggerFactory.GetExceptionLogger();
            userService = userServiceFactory.GetService();
            tokenService = tokenServiceFactory.GetService();
        }

        #region IUserFriendsService implementation
        public async Task AddSubscriber(string token, long subscriberID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                await mongoDBService.Connect();

                Models.UserFriends userFriends = await mongoDBService.Get(userID);
                userFriends.SubscribersID.Add(subscriberID);

                await mongoDBService.Update(userFriends, new[] { "SubscribersID" });
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while adding a new subscriber.");
            }
        }

        public async Task AddSubscription(string token, long subscriptionID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                await mongoDBService.Connect();

                Models.UserFriends userFriends = await mongoDBService.Get(userID);
                userFriends.SubscriptionsID.Add(subscriptionID);

                await mongoDBService.Update(userFriends, new[] { "SubscriptionsID" });
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while adding a new subscription.");
            }
        }

        public async Task DeleteSubscriber(string token, long subscriberID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                await mongoDBService.Connect();

                Models.UserFriends userFriends = await mongoDBService.Get(userID);
                userFriends.SubscribersID.Remove(subscriberID);

                await mongoDBService.Update(userFriends, new[] { "SubscribersID" });
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while deleting the subscriber.");
            }
        }

        public async Task DeleteSubscription(string token, long subscriptionID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                await mongoDBService.Connect();

                Models.UserFriends userFriends = await mongoDBService.Get(userID);
                userFriends.SubscriptionsID.Remove(subscriptionID);

                await mongoDBService.Update(userFriends, new[] { "SubscriptionsID" });
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while deleting the subscription");
            }
        }

        public async Task<IEnumerable<Subscriber>> GetSubscribers(string token)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));
                return await GetSubscribers(userID);
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting the subscribers");
            }
        }

        public async Task<IEnumerable<Subscriber>> GetSubscribers(long userID)
        {
            try
            {
                await mongoDBService.Connect();

                List<Subscriber> subscribers = new List<Subscriber>();
                Models.UserFriends userFriends = (await mongoDBService.Get(
                    new Dictionary<string, object>() { { "userID", userID } }))
                    .FirstOrDefault();

                if (userFriends == null)
                {
                    throw new ArgumentException("User with such id does not exist");
                }

                foreach (long subscriberID in userFriends.SubscribersID)
                {
                    UserInfo subUser = await userService.Get(subscriberID);
                    subscribers.Add(new Subscriber()
                    {
                        BirthDate = subUser.BirthDate,
                        Name = subUser.Name + " " + subUser.Surname
                    });
                }

                return subscribers;
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting the subscribers");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(string token)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));
                return await GetSubscriptions(userID);
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting the subscriptions");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(long userID)
        {
            try
            {
                await mongoDBService.Connect();

                List<Subscription> subscriptions = new List<Subscription>();
                Models.UserFriends userFriends = (await mongoDBService.Get(
                    new Dictionary<string, object>() { { "userID", userID } }))
                    .FirstOrDefault();

                if (userFriends == null)
                {
                    throw new ArgumentException("User with such id does not exist");
                }

                foreach (long subscriptionID in userFriends.SubscriptionsID)
                {
                    UserInfo subUser = await userService.Get(subscriptionID);
                    subscriptions.Add(new Subscription()
                    {
                        BirthDate = subUser.BirthDate,
                        Name = subUser.Name + " " + subUser.Surname
                    });
                }

                return subscriptions;
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while getting the subscriptions");
            }
        }
        #endregion
    }
}
