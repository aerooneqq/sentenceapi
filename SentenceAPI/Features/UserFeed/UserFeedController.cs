using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentenceAPI.ActionResults;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserFeed.Interfaces;

namespace SentenceAPI.Features.UserFeed
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserFeedController : Controller
    {
        private static LogConfiguration LogConfiguration => new LogConfiguration()
        {
            ControllerName = "UserFeedController",
            ServiceName = string.Empty
        };

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserFeedService userFeedService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private IUserFeedServiceFactory userFeedServiceFactory;
        #endregion

        public UserFeedController()
        {
            userFeedServiceFactory = (IUserFeedServiceFactory)factoriesManager[typeof(IUserFeedServiceFactory)];
            loggerFactory = (ILoggerFactory)factoriesManager[typeof(ILoggerFactory)];

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;

            userFeedService = userFeedServiceFactory.GetService();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                string token = GetToken(Request);

                var userFeed = await userFeedService.GetUserFeed(token);

                return new OkJson<IEnumerable<Models.UserFeed>>(userFeed);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                return new InternalServerError();
            }
        }

        private string GetToken(HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            return authHeader.Split()[1];
        }

        [HttpPut]
        public async Task<IActionResult> InsertFeed()
        {
            try
            {
                string token = GetToken(Request);

                string message = null;
                using (StreamReader sr = new StreamReader(Request.Body))
                {
                    message = await sr.ReadToEndAsync();
                }

                await userFeedService.InsertUserPost(token, message);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError();
            }
        }
    }
}
