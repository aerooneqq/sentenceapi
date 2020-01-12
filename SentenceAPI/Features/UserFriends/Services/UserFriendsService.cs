using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using SentenceAPI.Features.Authentication.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.UserFriends.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;
using MongoDB.Bson;

namespace SentenceAPI.Features.UserFriends.Services
{
    public class UserFriendsService : IUserFriendsService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<Models.UserFriends> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion  

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public UserFriendsService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserFriends>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(this.GetType());
        }


        #region IUserFriendsService implementation
        public async Task CreateUserFriendsRecord(ObjectId userID)
        {
            try 
            {
                var userFriends = new UserFriends.Models.UserFriends()
                {
                    ID = ObjectId.GenerateNewId(),
                    SubscribersID = new List<ObjectId>(),
                    SubscriptionsID = new List<ObjectId>(),
                    UserID = userID
                };

                await database.Connect().ConfigureAwait(false);
                await database.Insert(userFriends).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured when creating the user", ex);
            }
        }

        public async Task AddSubscriberAsync(string token, ObjectId subscriberID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                await database.Connect().ConfigureAwait(false);

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
                    userID))).FirstOrDefault();
                userFriends.SubscribersID.Add(subscriberID);

                await database.Update(userFriends, new[] { "SubscribersID" }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while adding a new subscriber.");
            }
        }

        public async Task AddSubscriptionAsync(string token, ObjectId subscriptionID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                await database.Connect().ConfigureAwait(false);

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
                    userID))).FirstOrDefault();

                userFriends.SubscriptionsID.Add(subscriptionID);

                await database.Update(userFriends, new[] { "SubscriptionsID" });
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while adding a new subscription.");
            }
        }

        public async Task DeleteSubscriberAsync(string token, ObjectId subscriberID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while deleting the subscriber.");
            }
        }

        public async Task DeleteSubscriptionAsync(string token, ObjectId subscriptionID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                await database.Connect();

                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
                    userID))).FirstOrDefault();

                userFriends.SubscriptionsID.Remove(subscriptionID);

                await database.Update(userFriends, new[] { "SubscriptionsID" });
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while deleting the subscription");
            }
        }

        public async Task<IEnumerable<Subscriber>> GetSubscribersAsync(string token)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));
                return await GetSubscribersAsync(userID);
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the subscribers");
            }
        }

        public async Task<IEnumerable<Subscriber>> GetSubscribersAsync(ObjectId userID)
        {
            try
            {
                await database.Connect();

                List<Subscriber> subscribers = new List<Subscriber>();
                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
                    userID))).FirstOrDefault();

                if (userFriends == null)
                {
                    throw new ArgumentException("User with such id does not exist");
                }

                foreach (ObjectId subscriberID in userFriends.SubscribersID)
                {
                    UserInfo subUser = await userService.GetAsync(subscriberID);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the subscribers");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(string token)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));
                return await GetSubscriptionsAsync(userID);
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the subscriptions");
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(ObjectId userID)
        {
            try
            {
                await database.Connect();

                List<Subscription> subscriptions = new List<Subscription>();
                Models.UserFriends userFriends = (await database.Get(new EqualityFilter<ObjectId>("userID",
                    userID))).FirstOrDefault();

                if (userFriends == null)
                {
                    throw new ArgumentException("User with such id does not exist");
                }

                foreach (ObjectId subscriptionID in userFriends.SubscriptionsID)
                {
                    UserInfo subUser = await userService.GetAsync(subscriptionID);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the subscriptions");
            }
        }
        #endregion
    }
}
