using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.Features.Users.Interfaces;


namespace SentenceAPI.Features.UserFeed
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserFeedController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IUserFeedService userFeedService;
        private readonly IRequestService requestService;
        private readonly IUserService<UserInfo> userService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public UserFeedController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IUserFeedService>().TryGetTarget(out userFeedService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);

            logConfiguration = new LogConfiguration(this.GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                string token = requestService.GetToken(Request);

                var userFeed = await userFeedService.GetUserFeedAsync(token);

                return new OkJson<UsersFeedDto>(userFeed);
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

        [HttpPut]
        public async Task<IActionResult> InsertFeed()
        {
            try
            {
                string token = requestService.GetToken(Request);
                var user = await userService.GetAsync(token).ConfigureAwait(false);

                if (user.Name is null || user.Surname is null)
                {
                    return new BadSentRequest<string>("Set your name and surname to insert the post."); 
                }

                string message = await requestService.GetRequestBody(Request);

                await userFeedService.InsertUserPostAsync(token, message).ConfigureAwait(false);

                return new Ok();
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
