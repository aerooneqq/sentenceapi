using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.Features.UserActivity.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.UserActivity.Factories;

using DataAccessLayer.DatabasesManager;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;

namespace SentenceAPI.Features.UserActivity.Services
{
    public class UserActivityService : IUserActivityService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<Models.UserActivity> database;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Constructors
        public UserActivityService()
        {
            databasesManager.MongoDBFactory.GetDatabase<Models.UserActivity>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }
        #endregion

        #region IUserActivityService implementation
        /// <summary>
        /// This method adds a single activity to a list of user's single activities with
        /// a given userID. If there is no record of user activity, then the new record is created
        /// in the database.
        /// </summary>
        /// <exception cref="DatabaseException">
        /// When the error occurs while working with the database. Provides the meaningful description of the error.
        /// </exception>
        public async Task AddSingleActivity(long userID, SingleUserActivity singleUserActivity)
        {
            await database.Connect();

            var filter = new EqualityFilter<long>("userID", userID); 
            Models.UserActivity userActivity = (await database.Get(filter).ConfigureAwait(false))
                .FirstOrDefault();

            if (userActivity == null)
            {
                await CreateUserActivity(userID);
            }

            userActivity = (await database.Get(filter).ConfigureAwait(false))
                .FirstOrDefault();

            AddActivityToList(userActivity, singleUserActivity);

            await database.Update(userActivity, new[] { "Activities" });
        }

        private void AddActivityToList(Models.UserActivity userActivity, SingleUserActivity singleUserActivity)
        {
            var activitiesList = userActivity.Activities.ToList();

            activitiesList.Add(singleUserActivity);
            userActivity.Activities = activitiesList;
        }

        /// <summary>
        /// Creates the user activity record in the database.
        /// </summary>
        private async Task CreateUserActivity(long userID)
        {
            await database.Insert(new Models.UserActivity()
            {
                Activities = new List<SingleUserActivity>(),
                IsOnline = false,
                LastActivityDate = DateTime.Now,
                LastOnline = DateTime.Now,
                UserID = userID
            });
        }

        /// <summary>
        /// Gets the user activity for a user with a given ID.
        /// </summary>
        /// <returns>
        /// Can return null if theere is no user with a given ID.
        /// </returns>
        public async Task<Models.UserActivity> GetUserActivity(long userID)
        {
            await database.Connect();

            var filter = new EqualityFilter<long>("userID", userID);
            return (await database.Get(filter).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the collection of the single user's activities with a given userID.
        /// </summary>
        /// <returns>
        /// Can return null if the user with a given ID does not exist.
        /// </returns>
        public async Task<IEnumerable<SingleUserActivity>> GetUserSingleActivities(long userID)
        {
            await database.Connect();
            var filter = new EqualityFilter<long>("userID", userID);
            return (await database.Get(filter).ConfigureAwait(false)).FirstOrDefault()?.Activities;
        }

        /// <summary>
        /// Updates the given properties of a userActivity in a mongo database.
        /// </summary>
        /// <param name="properties">
        /// The array of properties which must be updated.
        /// </param>
        public async Task UpdateActivity(Models.UserActivity userActivity, IEnumerable<string> properties)
        {
            await database.Connect();
            await database.Update(userActivity, properties);
        }
        #endregion
    }
}
