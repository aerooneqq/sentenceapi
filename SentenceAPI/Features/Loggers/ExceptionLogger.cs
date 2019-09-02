using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Loggers.Interfaces;

using Newtonsoft.Json;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;

namespace SentenceAPI.Features.Logger
{
    public class ExceptionLogger : ILogger<ApplicationError>
    {
        private static readonly object fileLocker = new object();
        private static readonly string databaseConfigFile = "mongo_database_config.json";

        #region Databases
        private IDatabaseService<ApplicationError> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
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
            databasesManager.MongoDBFactory.GetDatabase<ApplicationError>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            FileName = "log.txt";
        }
        #endregion

        #region ILogger implmentation
        public void WriteLogToFile(ApplicationError logObject)
        {
            lock (fileLocker)
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

        public async Task Log(ApplicationError logObject)
        {
            logObject.LogConfiguration = LogConfiguration;

            try
            {
                await database.Connect();
                await database.Insert(logObject);
            }
            catch
            {
                new Thread(new ThreadStart(() => WriteLogToFile(logObject))).Start();
            }
        }
        #endregion
    }
}
