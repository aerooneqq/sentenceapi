using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using SentenceAPI.Features.Middlewares.RequestLoggerMiddleware.Models;

using Newtonsoft.Json;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Configuration;

namespace SentenceAPI.Middlewares.RequestLoggerMiddleware
{
    /// <summary>
    /// Class which represents the middleware request logger.
    /// </summary>
    public class RequestLogger
    {
        #region Static fields
        private static readonly string fileName = "request_log.txt";
        private static readonly string databaseConfigFile = "mongo_database_config.json";
        #endregion

        #region Databases
        private IDatabaseService<RequestLog> database;
        private IConfigurationBuilder configurationBuilder;
        private DatabasesManager databasesManager = DatabasesManager.Manager;
        #endregion

        #region Fields
        private RequestDelegate nextMiddlewareDel;
        #endregion

        #region Constructors
        public RequestLogger(RequestDelegate nextMiddlewareDel)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;

            databasesManager.MongoDBFactory.GetDatabase<RequestLog>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }
        #endregion

        /// <summary>
        /// Tries to log the request in the mongo database. If any exception occurs then this method logs
        /// the request in a request_log.txt. After all these processes the next middleware component is called.
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            RequestLog requestLog = new RequestLog(httpContext.Request);

            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Insert(requestLog).ConfigureAwait(false);
            }
            catch
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(requestLog));
                    }
                }
            }
            finally
            {
                await nextMiddlewareDel.Invoke(httpContext).ConfigureAwait(false);
            }
        }
    }
}
