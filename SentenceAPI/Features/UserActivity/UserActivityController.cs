using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Features.UserActivity.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Exceptions;
using SharedLibrary.ActionResults;

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
        private IFactoriesManager factoriesManager = ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public UserActivitiesController()
        {
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            exceptionLogger.LogConfiguration = LogConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserActivities()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                long id = long.Parse(tokenService.GetTokenClaim(token, "ID"));

                var userActivities = await userActivityService.GetUserActivityAsync(id).ConfigureAwait(false);

                return new OkJson<Models.UserActivity>(userActivities);
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
    }
}
