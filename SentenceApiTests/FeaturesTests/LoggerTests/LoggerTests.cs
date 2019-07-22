using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using NUnit.Framework;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Factories;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;

namespace SentenceApiTests.FeaturesTests.LoggerTests
{
    [TestFixture]
    public class LoggerTests
    {
        #region Services
        private ILogger logger;
        private IMongoDBService<ApplicationError> mongoDBService;
        #endregion

        #region Factory
        private IMongoDBServiceFactory mongoDBServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<ApplicationError> mongoDBServiceBuilder;
        #endregion

        [SetUp]
        public void SetUp()
        {
            loggerFactory = new LoggerFactory();
            mongoDBServiceFactory = new MongoDBServiceFactory();

            logger = loggerFactory.GetLogger();
            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService<ApplicationError>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").SetConnectionString()
                .SetDatabaseName("SentenceDatabase").SetCollectionName().Build();
        }

        [Test]
        public async Task TestLogException()
        {
            try
            {
                Exception ex = new Exception("TestMessage");

                LogConfiguration logConfiguration = new LogConfiguration()
                {
                    ControllerName = "TestControllerName",
                    ServiceName = "TestServiceName"
                };

                ApplicationError applicationError = new ApplicationError(ex.Message, logConfiguration);

                logger.LogConfiguration = logConfiguration;
                await logger.Log(ex);

                await mongoDBService.Connect();
                var errs = (await mongoDBService.Get(new Dictionary<string, object>()
                {
                    { "message", ex.Message },
                    { "configuration.controllerName", logConfiguration.ControllerName },
                    { "configuration.serviceName", logConfiguration.ServiceName }
                })).ToList();

                if (errs == null || errs.Count != 1 || errs[0].Message != ex.Message || 
                    errs[0].LogConfiguration.ControllerName != logConfiguration.ControllerName ||
                    errs[0].LogConfiguration.ServiceName != logConfiguration.ServiceName)
                {
                    Assert.Fail();
                }

                Assert.Pass();
            }
            catch (Exception ex) when (!(ex is SuccessException))
            {
                Assert.Fail(ex.Message);
            }

            Assert.Pass();
        }
    }
}
