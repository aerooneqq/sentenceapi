using Microsoft.AspNetCore.Http;

using SentenceAPI.ApplicationFeatures.Requests.Interfaces;

using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using MongoDB.Bson;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace SentenceAPI.ApplicationFeatures.Requests
{
    public class RequestService : IRequestService
    {
        #region Static properties
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Databases
        private readonly IDatabaseService<RequestLog> database;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public RequestService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            
            databaseManager.MongoDBFactory.GetDatabase<RequestLog>().TryGetTarget(out database);

            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
            
            logConfiguration = new LogConfiguration(GetType());
        }
        

        public async Task<string> GetRequestBody(HttpRequest request)
        {
            using StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);

            return await sr.ReadToEndAsync();
        }

        public async Task<T> GetRequestBody<T>(HttpRequest request)
        { 
            using StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            string body = await sr.ReadToEndAsync().ConfigureAwait(false);

            IDeserializer<T> deserializer = new JsonDeserializer<T>(body);
            return deserializer.Deserialize();
        }

        public string GetToken(HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            return authHeader.Split()[1];
        }

        public async Task<ObjectId> LogRequestToDatabase(HttpRequest request)
        {
            try
            {
                RequestLog requestLog = new RequestLog(request);

                await database.Connect().ConfigureAwait(false);
                await database.Insert(requestLog).ConfigureAwait(false); 

                return requestLog.ID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration); 
                throw new DatabaseException("The error occured while logging request", ex);
            }
        }
    }
}
