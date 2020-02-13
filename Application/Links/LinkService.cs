using System;
using System.Linq;
using System.Threading.Tasks;

using Application.Links.Interfaces;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.Links;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;


namespace Application.Links.Services
{
    public class LinkService : ILinkService
    {
        #region Static fields
        private static Random Random { get; } = new Random();
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion


        #region Daatabases
        private IDatabaseService<VerificationLink> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion


        #region Service
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public LinkService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<VerificationLink>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(this.GetType());
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }
        #endregion
    }
}
