using System;
using System.Linq;
using System.Threading.Tasks;

using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.Links.Models;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Features.Users.Models;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using SharedLibrary.Loggers.Configuration;

namespace SentenceAPI.Features.Links.Services
{
    public class LinkService : ILinkService
    {
        #region Static fields
        private static Random Random { get; } = new Random();
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Daatabases
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        private IDatabaseService<VerificationLink> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Service
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public LinkService()
        {
            databasesManager.MongoDBFactory.GetDatabase<VerificationLink>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
        }

        #region ILinkService implementation
        public async Task<string> CreateVerificationLinkAsync(UserInfo user)
        {
            try
            {
                VerificationLink link = new VerificationLink(user);

                await database.Connect();
                await database.Insert(link);

                return link.Link;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
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
        public async Task<bool?> ActivateLinkAsync(string link)
        {
            try
            {
                var filter = new EqualityFilter<string>("link", link);
                VerificationLink verificationLink = (await database.Get(filter)).FirstOrDefault();

                if (verificationLink == null)
                {
                    return null;
                }

                if (verificationLink.Used)
                {
                    return false;
                }

                verificationLink.Used = true;
                await database.Update(verificationLink, new[] { "Used" });

                return true;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                throw new DatabaseException("Error occured while working with the database");
            }
        }
        #endregion
    }
}
