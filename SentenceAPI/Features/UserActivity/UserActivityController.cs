using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserActivity.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserActivity
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserActivityController : Controller
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserActivityService userActivityService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private IUserActivityServiceFactory userActivityServiceFactory;
        #endregion

        public UserActivityController()
        {
            userActivityServiceFactory = factoriesManager[typeof(IUserActivityServiceFactory)].Factory
                as IUserActivityServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            userActivityService = userActivityServiceFactory.GetService();
            exceptionLogger = loggerFactory.GetExceptionLogger();
        }

        public async Task<IActionResult> GetUserActivities([FromQuery]long id)
        {
            try
            {
                var userActivities = await userActivityService.GetUserActivity(id);
                return Ok(JsonConvert.SerializeObject(userActivities));
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }
    }
}
