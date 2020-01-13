using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;

using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;
using SharedLibrary.Loggers.Models;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;

using MongoDB.Bson;

using SharedLibrary.Loggers.Configuration;

using DataAccessLayer.Filters;


namespace DocumentsAPI.ApplicationFeatures.Requests
{
    public class RequestService : IRequestService
    {
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
                var request = await database.Get(new EqualityFilter<ObjectId>("_id", requestID)).ConfigureAwait(false);

                return !(request is null);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("");
            }
        }
    }
}
