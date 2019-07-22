using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using NUnit.Framework;

using SentenceAPI.Databases.MongoDB;
using SentenceAPI.Features.UserActivity.Factories;
using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.Features.UserActivity.Models;

namespace SentenceApiTests.FeaturesTests.UserActivityTests
{
    [TestFixture]
    class UserActivityTests
    {
        #region Services
        private IUserActivityService userActivityService;
        #endregion

        #region
        private IUserActivityServiceFactory userActivityServiceFactory;
        #endregion

        [SetUp]
        public void SetUp()
        {
            userActivityServiceFactory = new UserActivityServiceFactory();
            userActivityService = userActivityServiceFactory.GetService();
        }

        [Test]
        public async Task TestGettingTheActivity()
        {
            long id = -1;

            var activity = await userActivityService.GetUserActivity(id);

            if (activity != null)
            {
                Assert.Fail();
            }

            id = 0;
            activity = await userActivityService.GetUserActivity(id);

            if (activity == null || activity.ID != 0)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestGettingUserSingleActivity()
        {
            long id = -1;
        
            var activities = await userActivityService.GetUserSingleActivities(id);
            if (activities != null)
            {
                Assert.Fail();
            }

            id = 0;
            activities = await userActivityService.GetUserSingleActivities(id);

            if (activities == null)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestUpdatingActivity()
        {
            var userActivity = await userActivityService.GetUserActivity(0);
            userActivity.IsOnline = true;

            await userActivityService.UpdateActivity(userActivity,
                new[] { "IsOnline", "LastActivityDate" });
            userActivity = await userActivityService.GetUserActivity(0);
            if (userActivity.IsOnline != true)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestAddingSingleActivity()
        {
            SingleUserActivity userSingleActivity = new SingleUserActivity()
            {
                Activity = "Special name which unique test",
                ActivityDate = DateTime.UtcNow
            };

            await userActivityService.AddSingleActivity(0, userSingleActivity);
            var userActivity = await userActivityService.GetUserActivity(0);

            if (userActivity.Activities.ToList().FindIndex(sa => sa.Activity == 
                userSingleActivity.Activity) == -1)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
