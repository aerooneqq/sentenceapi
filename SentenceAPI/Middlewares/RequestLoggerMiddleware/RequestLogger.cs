using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Middlewares.RequestLoggerMiddleware.Models;

using Newtonsoft.Json;
using SentenceAPI.Databases.CommonInterfaces;

namespace SentenceAPI.Middlewares.RequestLoggerMiddleware
{
    /// <summary>
    /// Class which represents the middleware request logger.
    /// </summary>
    public class RequestLogger
    {
        #region Constants
        private const string FileName = "request_log.txt";
        #endregion

        #region Services
        private IDatabaseService<RequestLog> mongoDBService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        private IMongoDBServiceFactory mongoDBServiceFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<RequestLog> mongoDBServiceBuilder;
        #endregion

        #region Fields
        private RequestDelegate nextMiddlewareDel;
        #endregion

        #region Constructors
        public RequestLogger(RequestDelegate nextMiddlewareDel)
        {
            this.nextMiddlewareDel = nextMiddlewareDel;

            mongoDBServiceFactory = factoriesManager[typeof(IMongoDBServiceFactory)].Factory
                as IMongoDBServiceFactory;
            mongoDBServiceBuilder = mongoDBServiceFactory.GetBuilder(
                mongoDBServiceFactory.GetService<RequestLog>());
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json")
                .SetConnectionString().SetDatabaseName("SentenceDatabase").SetCollectionName().Build();
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
                await mongoDBService.Connect();
                await mongoDBService.Insert(requestLog);
            }
            catch
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {   
                        await sw.WriteLineAsync(JsonConvert.SerializeObject(requestLog));
                    }
                }
            }
            finally
            {
                await nextMiddlewareDel.Invoke(httpContext);
            }
        }
    }
}
