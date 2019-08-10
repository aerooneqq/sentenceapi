using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.Features.UserActivity.Models;
using SentenceAPI.Databases.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.Features.UserActivity.Factories;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.Filters;

namespace SentenceAPI.Features.UserActivity.Services
{
    public class UserActivityService : IUserActivityService
    {
        #region Services
        private IMongoDBService<Models.UserActivity> mongoDBService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Bulders
        private IMongoDBServiceBuilder<Models.UserActivity> mongoDBServiceBuilder;
        #endregion

        #region Constructors
        public UserActivityService()
        {
            mongoDBServiceFactory = new MongoDBServiceFactory();
            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService
                <Models.UserActivity>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString()
                .SetDatabaseName("SentenceDatabase")
                .SetCollectionName()
                .Build();

            mongoDBService.Connect().GetAwaiter().GetResult();
            mongoDBService.Insert(new Models.UserActivity()
            {
                IsOnline = false,
                UserID = 0,
                Activities = new List<SingleUserActivity>()
                {
                    new SingleUserActivity()
                    {
                        Activity = "Logged in",
                        ActivityDate = DateTime.Now,
                    },
                    new SingleUserActivity()
                    {
                        ActivityDate = DateTime.Now,
                        Activity = "Logged out"
                    }
                },
                LastActivityDate = DateTime.Now,
                LastOnline = DateTime.Now
            });
        }
        #endregion

        #region IUserActivityService implementation
        /// <summary>
        /// This method adds a single activity to a list of user's single activities with
        /// a given userID.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// When the error occurs while working with the database.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When there is no user with a given ID.
        /// </exception>
        public async Task AddSingleActivity(long userID, SingleUserActivity singleUserActivity)
        {
            await mongoDBService.Connect();
            var filter = new EqualityFilter<long>("userID", userID); 
            Models.UserActivity userActivity = (await mongoDBService.Get(filter).ConfigureAwait(false)).FirstOrDefault();

            if (userActivity == null)
            {
                throw new ArgumentNullException("The user activity for a user with such userID was not found");
            }

            var activitiesList = userActivity.Activities.ToList();
            activitiesList.Add(singleUserActivity);
            userActivity.Activities = activitiesList;
            await mongoDBService.Update(userActivity, new[] { "Activities" });
        }

        /// <summary>
        /// Gets the user activity for a user with a given ID.
        /// </summary>
        /// <returns>
        /// Can return null if theere is no user with a given ID.
        /// </returns>
        public async Task<Models.UserActivity> GetUserActivity(long userID)
        {
            await mongoDBService.Connect();

            var filter = new EqualityFilter<long>("userID", userID);
            return (await mongoDBService.Get(filter).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the collection of the single user's activities with a given userID.
        /// </summary>
        /// <returns>
        /// Can return null if the user with a given ID does not exist.
        /// </returns>
        public async Task<IEnumerable<SingleUserActivity>> GetUserSingleActivities(long userID)
        {
            await mongoDBService.Connect();
            var filter = new EqualityFilter<long>("userID", userID);
            return (await mongoDBService.Get(filter).ConfigureAwait(false)).FirstOrDefault()?.Activities;
        }

        /// <summary>
        /// Updates the given properties of a userActivity in a mongo database.
        /// </summary>
        /// <param name="properties">
        /// The array of properties which must be updated.
        /// </param>
        public async Task UpdateActivity(Models.UserActivity userActivity, IEnumerable<string> properties)
        {
            await mongoDBService.Connect();
            await mongoDBService.Update(userActivity, properties);
        }
        #endregion
    }
}
