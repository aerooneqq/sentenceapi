using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SentenceAPI.Databases.CommonInterfaces;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;

namespace SentenceAPI.Features.Loggers
{
    public class EmailLogger : ILogger<EmailLog>
    {
        private static object fileLoceker = new object();

        #region Services
        private IDatabaseService<EmailLog> mongoDBService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<EmailLog> mongoDBServiceBuilder;
        #endregion

        #region Properties
        public string FileName { get; }
        /// <summary>
        /// This property must be initialized befote each logging
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        public EmailLogger()
        {
            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory as IMongoDBServiceFactory;

            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(mongoDBServiceFactory.GetService<EmailLog>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString().SetDatabaseName("SentenceDatabase").SetCollectionName().Build();

            FileName = "email_log.txt";
        }

        #region ILogger implementation
        public void WriteLogToFile(EmailLog logObject)
        {
            lock (fileLoceker)
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Append, FileAccess.Write))
                {
                    string applicationErrorJson = JsonConvert.SerializeObject(logObject);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(applicationErrorJson);
                    }
                }
            }
        }

        public async Task Log(EmailLog logObject)
        {
            logObject.LogConfiguration = LogConfiguration;

            try
            {
                await mongoDBService.Connect();
                await mongoDBService.Insert(logObject);
            }
            catch
            {
                WriteLogToFile(logObject);
            }
        }
        #endregion
    }
}
