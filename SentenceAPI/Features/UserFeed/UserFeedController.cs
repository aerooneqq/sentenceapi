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
using SentenceAPI.Features.Requests.Interfaces;
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
        private IRequestService requestService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private IUserFeedServiceFactory userFeedServiceFactory;
        private IRequestServiceFactory requestServiceFactory;
        #endregion

        public UserFeedController()
        {
            userFeedServiceFactory = (IUserFeedServiceFactory)factoriesManager[typeof(IUserFeedServiceFactory)];
            loggerFactory = (ILoggerFactory)factoriesManager[typeof(ILoggerFactory)];
            requestServiceFactory = (IRequestServiceFactory)factoriesManager[typeof(IRequestServiceFactory)];

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;

            userFeedService = userFeedServiceFactory.GetService();
            requestService = requestServiceFactory.GetService();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                string token = requestService.GetToken(Request);

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

        [HttpPut]
        public async Task<IActionResult> InsertFeed()
        {
            try
            {
                string token = requestService.GetToken(Request);

                string message = requestService.GetRequestBody(Request);

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
