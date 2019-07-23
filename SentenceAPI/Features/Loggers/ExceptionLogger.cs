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
    public class ExceptionLogger : ILogger<ApplicationError>
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
        public string FileName { get; }
        #endregion

        #region Constructors
        public ExceptionLogger()
        {
            if (factoriesManager != null && (factoriesManager[typeof(IMongoDBServiceFactory)]
                is IMongoDBServiceFactory))
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

            FileName = "log.txt";
        }
        #endregion

        #region ILogger implmentation
        public async Task WriteLogToFile(ApplicationError logObject)
        {
            using (FileStream fs = new FileStream(FileName, FileMode.Append, FileAccess.Write))
            {
                string applicationErrorJson = JsonConvert.SerializeObject(logObject);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    await sw.WriteLineAsync(applicationErrorJson);
                }
            }
        }

        public async Task Log(ApplicationError logObject)
        {
            logObject.LogConfiguration = LogConfiguration;

            try
            {
                await mongoDBService.Connect();
                await mongoDBService.Insert(logObject);
            }
            catch
            {
                await WriteLogToFile(logObject);
            }
        }
        #endregion
    }
}
