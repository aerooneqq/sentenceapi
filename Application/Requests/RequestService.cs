using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Application.Requests.Exceptions;
using Application.Requests.Interfaces;
using Application.Responses.Interfaces;
using Application.Tokens.Interfaces;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Http;

using MongoDB.Bson;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Serialization;
using SharedLibrary.Serialization.Json;


namespace Application.Requests
{
    public class RequestService : IRequestService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";
        
        private readonly IDatabaseService<RequestLog> requestDatabase;
        
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IResponseService responseService;
        private readonly ITokenService tokenService;


        private readonly LogConfiguration logConfiguration;
        
        public RequestService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IResponseService>().TryGetTarget(out responseService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            
            databaseManager.MongoDBFactory.GetDatabase<RequestLog>().TryGetTarget(out requestDatabase);
            var configurationBuilder = new MongoConfigurationBuilder(requestDatabase.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }
        
        /// <summary>
        /// Sends a get request to a given url
        /// </summary>
        /// <exception cref="RequestException">When an error happens during the request execution</exception>
        public async Task<string> Get(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                HttpWebResponse response = (HttpWebResponse) (await request.GetResponseAsync());

                Stream responseStream = response.GetResponseStream();

                if (responseStream is null)
                {
                    return null;
                }

                using StreamReader sr = new StreamReader(response.GetResponseStream());

                return await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                throw new RequestException("The error occured while sending Get request", ex);
            }
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

                await requestDatabase.Connect().ConfigureAwait(false);
                await requestDatabase.Insert(requestLog).ConfigureAwait(false); 

                return requestLog.ID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration); 
                throw new DatabaseException("The error occured while logging request", ex);
            }
        }
        
        public async Task RedirectRequest(HttpContext context, string destinationServer)
        {
            ObjectId requestId = await LogRequestToDatabase(context.Request);
            
            HttpWebResponse response = await RedirectRequest(context.Request, requestId, destinationServer);
                    
            await responseService.ResponseCopier.Copy(response).To(context.Response);
        }
        
        private static async Task<HttpWebResponse> RedirectRequest(HttpRequest requestInfo, ObjectId requestId, 
            string url)
        {
            try
            {
                HttpWebRequest request = await GetRequestFrom(requestInfo, requestId, url);
                return (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                throw new RequestException("The error occured while sending the request", ex);
            }
        }
        
        private static async Task<HttpWebRequest> GetRequestFrom(HttpRequest requestInfo, ObjectId requestID, string destinationHost)
        {
            HttpWebRequestBuilder builder = new HttpWebRequestBuilder(requestInfo, destinationHost);
            
            return (await builder.SetHeaders(requestID).SetContent()).HttpWebRequest;
        }
    }
}