using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.FactoriesManager.Models;
using SentenceAPI.Features.Loggers.Factories;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

namespace SentenceApiTests.FeaturesTests.UserTests
{
    [TestFixture]
    public class UserServiceTests
    {
        #region Services
        private IUserService<UserInfo> userService;
        #endregion

        #region Factories
        private IUserServiceFactory userServiceFactory;
        #endregion

        [SetUp]
        public void SetUp()
        {
            FactoriesManager factoriesManager = FactoriesManager.Instance;

            factoriesManager.AddFactory(new FactoryInfo(new UserServiceFactory(),
                typeof(IUserServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new MongoDBServiceFactory(),
                typeof(IMongoDBServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(),
                typeof(ILoggerFactory)));
            userServiceFactory = new UserServiceFactory();
            userService = userServiceFactory.GetService();
        }

        [Test]
        public async Task TestNewUserCreation()
        {
            UserInfo userInfo = new UserInfo()
            {
                Email = "stepanov-ev@yandex.ru",
                Password = "1234567"
            };

            long id = await userService.CreateNewUser(userInfo.Email, userInfo.Password);
            Assert.Pass();
        }
    }
}
