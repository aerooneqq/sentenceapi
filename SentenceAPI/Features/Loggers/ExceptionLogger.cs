using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Features.Loggers.Interfaces;

using Newtonsoft.Json;

namespace SentenceAPI.Features.Logger
{
    public class ExceptionLogger : ILogger
    {
        #region Services
        private IMongoDBService<ApplicationError> mongoDBService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Builder
        private IMongoDBServiceBuilder<ApplicationError> mongoDBServiceBuilder;
        #endregion

        #region Properties
        /// <summary>
        /// This property must be initialized before logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        public string LogFileName { get; }
        #endregion

        #region Constructors
        public ExceptionLogger()
        {
            if (factoriesManager != null && (factoriesManager[typeof(IMongoDBServiceFactory)] is IMongoDBServiceFactory))
            {
                mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory as
                    IMongoDBServiceFactory;
            }
            else
            {
                mongoDBServiceFactory = new MongoDBServiceFactory();
            }

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService
                <ApplicationError>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString().SetDatabaseName("SentenceDatabase").SetCollectionName().Build();

            LogFileName = "log.txt";
        }
        #endregion

        #region ILogger implmentation
        /// <summary>
        /// Tries to log the exception into the database, if any exception occures then the method
        /// logs the exception in the txt file on the server.
        /// </summary>
        public async Task Log(Exception ex)
        {
            try
            {
                await mongoDBService.Connect();
                await mongoDBService.Insert(new ApplicationError(ex.Message, LogConfiguration));
            }
            catch
            {
                await WriteLogToFile(ex.Message);
            }
        }

        /// <summary>
        /// Tries to log the message into the database, if any exception occures then the method
        /// logs the exception in the txt file on the server.
        /// </summary>
        public async Task Log(string message)
        {
            try
            {
                await mongoDBService.Connect();
                await mongoDBService.Insert(new ApplicationError(message, LogConfiguration));
            }
            catch
            {
                await WriteLogToFile(message);
            }
        }

        private async Task WriteLogToFile(string message)
        {
            using (FileStream fs = new FileStream(LogFileName, FileMode.Append, FileAccess.Write))
            {
                string applicationErrorJson = JsonConvert.SerializeObject(new ApplicationError(message, LogConfiguration));
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    await sw.WriteLineAsync(applicationErrorJson);
                }
            }
        }
        #endregion
    }
}
