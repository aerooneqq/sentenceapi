using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using SentenceAPI.Databases.MongoDB;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.FactoriesManager.Models;
using SentenceAPI.Features.Links.Factories;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Models;
using SentenceAPI.Features.Loggers.Factories;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Response.Factories;
using SentenceAPI.Features.Response.Interfaces;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

namespace SentenceApiTests.FeaturesTests.LinkServicesTest
{
    [TestFixture]
    public class LinkServicesTests
    {
        #region Services
        private ILinkService linkService;
        private IUserService<UserInfo> userService;
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = FactoriesManager.Instance;

        private IUserServiceFactory userServiceFactory;
        private ILinkServiceFactory linkServiceFactory;
        #endregion

        [SetUp]
        public void SetUp()
        {
            factoriesManager.AddFactory(new FactoryInfo(new LinkServiceFactory(),
                typeof(ILinkServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserServiceFactory(),
                typeof(IUserServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new MongoDBServiceFactory(),
                typeof(IMongoDBServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(),
                typeof(ILoggerFactory)));

            userServiceFactory = factoriesManager[typeof(IUserServiceFactory)].Factory as IUserServiceFactory;
            userService = userServiceFactory.GetService();

            linkServiceFactory = factoriesManager[typeof(ILinkServiceFactory)].Factory as ILinkServiceFactory;
            linkService = linkServiceFactory.GetService();
        }

        [Test]
        public async Task TestVerificationLinkCreation()
        {
            var user = await userService.Get(0);
            await linkService.CreateVerificationLink(user);
        }
    }
}
