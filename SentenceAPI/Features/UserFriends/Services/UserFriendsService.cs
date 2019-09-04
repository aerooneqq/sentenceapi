using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
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
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<Models.UserFriends> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ITokenServiceFactory tokenServiceFactory;
        private IUserServiceFactory userServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        public UserFriendsService()
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserFriends>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        #region IUserFriendsService implementation
        public async Task AddSubscriber(string token, long subscriberID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();
                userFriends.SubscribersID.Add(subscriberID);

                await database.Update(userFriends, new[] { "SubscribersID" });
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

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();

                userFriends.SubscriptionsID.Add(subscriptionID);

                await database.Update(userFriends, new[] { "SubscriptionsID" });
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

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();

                userFriends.SubscribersID.Remove(subscriberID);

                await database.Update(userFriends, new[] { "SubscribersID" });
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

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();

                await database.Update(userFriends, new[] { "SubscriptionsID" });
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
                await database.Connect();

                List<Subscriber> subscribers = new List<Subscriber>();
                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();

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
                await database.Connect();

                List<Subscription> subscriptions = new List<Subscription>();
                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<long>("userID",
                    userID))).FirstOrDefault();

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
