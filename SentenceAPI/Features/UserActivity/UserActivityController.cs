using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.FactoriesManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Exceptions;
using SentenceAPI.ActionResults;

namespace SentenceAPI.Features.UserActivity
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserActivitiesController : Controller
    {
        private static LogConfiguration LogConfiguration => new LogConfiguration()
        {
            ControllerName = "UserActivitiesController",
            ServiceName = string.Empty
        };

        #region Services
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IUserActivityService userActivityService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ITokenServiceFactory tokenServiceFactory;
        private ILoggerFactory loggerFactory;
        private IUserActivityServiceFactory userActivityServiceFactory;
        #endregion

        public UserActivitiesController()
        {
            userActivityServiceFactory = factoriesManager[typeof(IUserActivityServiceFactory)] as IUserActivityServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)] as ILoggerFactory;
            tokenServiceFactory = factoriesManager[typeof(ITokenServiceFactory)] as ITokenServiceFactory;

            userActivityService = userActivityServiceFactory.GetService();
            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = LogConfiguration;
            tokenService = tokenServiceFactory.GetService();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserActivities()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                long id = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                var userActivities = await userActivityService.GetUserActivity(id);

                return new OkJson<Models.UserActivity>(userActivities);
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
