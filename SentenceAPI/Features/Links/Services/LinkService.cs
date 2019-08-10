using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Databases.Filters;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Users.Models;

namespace SentenceAPI.Features.Links.Services
{
    public class LinkService : ILinkService
    {
        #region Static fields
        public static Random Random { get; } = new Random();
        public static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "LinkService"
        };
        #endregion

        #region Service
        private ILogger<ApplicationError> exceptionLogger;
        private IDatabaseService<VerificationLink> mongoDBService;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<VerificationLink> mongoDBServiceBuilder;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        public LinkService()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory 
                as IMongoDBServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory
                as ILoggerFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(
                mongoDBServiceFactory.GetService<VerificationLink>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString().SetDatabaseName("SentenceDatabase").SetCollectionName().
                Build();

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;
        }

        #region ILinkService implementation
        public async Task<string> CreateVerificationLink(UserInfo user)
        {
            try
            {
                VerificationLink link = new VerificationLink(user);

                await mongoDBService.Connect();
                await mongoDBService.CreateCollection();
                await mongoDBService.Insert(link);

                return link.Link;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        /// <summary>
        /// Performs an activation of the verification link.
        /// </summary>
        /// <returns>
        /// If the link does not exist the method returns null.
        /// If the link is already activated the method returns false.
        /// If the link exists and was not activated the method returns true.
        /// </returns>
        public async Task<bool?> ActivateLink(string link)
        {
            try
            {
                var filter = new EqualityFilter<string>("link", link);
                VerificationLink verificationLink = (await mongoDBService.Get(filter)).FirstOrDefault();

                if (verificationLink == null)
                {
                    return null;
                }

                if (verificationLink.Used)
                {
                    return false;
                }

                verificationLink.Used = true;
                await mongoDBService.Update(verificationLink, new[] { "Used" });

                return true;
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                throw new DatabaseException("Error occured while working with the database");
            }
        }
        #endregion
    }
}
