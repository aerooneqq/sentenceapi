using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;

namespace SentenceAPI.Features.Loggers
{
    public class EmailLogger : ILogger<EmailLog>
    {
        #region Static fields
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        private static object fileLoceker = new object();
        #endregion

        #region Databases
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        private IDatabaseService<EmailLog> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
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
            databasesManager.MongoDBFactory.GetDatabase<EmailLog>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

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
                await database.Connect();
                await database.Insert(logObject);
            }
            catch
            {
                WriteLogToFile(logObject);
            }
        }
        #endregion
    }
}
