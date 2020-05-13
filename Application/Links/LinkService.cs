using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using MongoDB.Bson;


namespace Application.Links.Services
{
    public class LinkService : ILinkService
    {
        #region Static fields
        private static Random Random { get; } = new Random();
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion
        
        #region Daatabases
        private readonly IDatabaseService<VerificationLink> verificationLinksDatabase;
        private readonly IDatabaseService<WordDownloadLink> wordLinksDatabase;
            #endregion

            #region Service
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public LinkService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            databaseManager.MongoDBFactory.GetDatabase<VerificationLink>().TryGetTarget(out verificationLinksDatabase);
            databaseManager.MongoDBFactory.GetDatabase<WordDownloadLink>().TryGetTarget(out wordLinksDatabase);

            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(verificationLinksDatabase.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
            
            configurationBuilder = new MongoConfigurationBuilder(wordLinksDatabase.Configuration);
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

                await verificationLinksDatabase.Connect();
                await verificationLinksDatabase.Insert(link);

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
                VerificationLink verificationLink = (await verificationLinksDatabase.Get(filter)).FirstOrDefault();

                if (verificationLink == null)
                {
                    return null;
                }

                if (verificationLink.Used)
                {
                    return false;
                }

                verificationLink.Used = true;
                await verificationLinksDatabase.Update(verificationLink, new[] { "Used" });

                return true;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }
        
        public async Task MarkWordLinkAsUsed(ObjectId linkID)
        {
            try
            {
                await wordLinksDatabase.Connect().ConfigureAwait(false);
                
                var filter = new EqualityFilter<ObjectId>("_id", linkID);
                var wordDownloadLink = (await wordLinksDatabase.Get(filter).ConfigureAwait(false))
                    .FirstOrDefault();
                if (wordDownloadLink is null)
                {
                    throw new ArgumentException("No link was found for given ID");
                }

                wordDownloadLink.Used = true;

                await wordLinksDatabase.Update(wordDownloadLink).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<WordDownloadLink> GetUnusedDownloadLink(ObjectId linkID)
        {
            try
            {
                await wordLinksDatabase.Connect().ConfigureAwait(false);
                
                var filter = new EqualityFilter<ObjectId>("_id", linkID);
                var wordDownloadLink = (await wordLinksDatabase.Get(filter).ConfigureAwait(false))
                    .FirstOrDefault();
                if (wordDownloadLink is null)
                {
                    throw new ArgumentException("No link was found for given ID");
                }

                if (wordDownloadLink.Used)
                {
                    throw new ArgumentException("The link has already been used");
                }

                return wordDownloadLink;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }

        public async Task<WordDownloadLink> CreateWordDownloadLink(ObjectId documentID, ObjectId userID)
        {
            try
            {
                await wordLinksDatabase.Connect().ConfigureAwait(false);
                var wordDownloadLink = new WordDownloadLink(userID, documentID);

                await wordLinksDatabase.Insert(wordDownloadLink).ConfigureAwait(false);

                return wordDownloadLink;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while working with the database");
            }
        }
        #endregion
    }
}
