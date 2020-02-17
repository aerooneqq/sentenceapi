using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;

using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;

using MongoDB.Bson;

using DataAccessLayer.Filters;

using Domain.Logs;
using Domain.Logs.Configuration;

using Application.Tokens.Interfaces;


namespace DocumentsAPI.ApplicationFeatures.Requests
{
    public class RequestService : IRequestService
    {
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
    
        #region Databases
        private readonly IDatabaseService<RequestLog> database;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public RequestService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            databaseManager.MongoDBFactory.GetDatabase<RequestLog>().TryGetTarget(out database);
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);

            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetUserName().SetPassword()
                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();
            
            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            using StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);

            return await sr.ReadToEndAsync();
        }

        public async Task<T> GetRequestBodyAsync<T>(HttpRequest request)
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

        public async Task<bool> CheckIfRequestInDatabase(ObjectId requestID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var request = await database.Get(new EqualityFilter<ObjectId>("_id",
                    requestID)).ConfigureAwait(false);

                return !(request is null);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("");
            }
        }

        public ObjectId GetUserID(HttpRequest request)
        {
            string token = GetToken(request);
            return ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));
        }
    }
}
