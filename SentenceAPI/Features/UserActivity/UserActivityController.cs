using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.ActionResults;

using System;
using System.Threading.Tasks;
using Application.Tokens.Interfaces;
using Application.UserActivity.Interfaces;
using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using MongoDB.Bson;


namespace SentenceAPI.Features.UserActivity
{
    [Route("sentenceapi/[controller]"), Authorize, ApiController]
    public class UserActivitiesController : Controller
    {
        #region Services
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IUserActivityService userActivityService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public UserActivitiesController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(this.GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetUserActivities()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                ObjectId id = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                var userActivities = await userActivityService.GetUserActivityAsync(id).ConfigureAwait(false);

                return new OkJson<Domain.UserActivity.UserActivity>(userActivities);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}
