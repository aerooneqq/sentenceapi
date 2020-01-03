using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.Authentication.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Features.UserActivity.Interfaces;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using System;
using System.Threading.Tasks;
using DataAccessLayer.Exceptions;
using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Configuration;

namespace SentenceAPI.Features.UserActivity
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserActivitiesController : Controller
    {
        #region Services
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IUserActivityService userActivityService;
        #endregion


        public UserActivitiesController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}
