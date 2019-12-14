using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using DataAccessLayer.Exceptions;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;

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
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public UserFeedController()
        {
            factoriesManager.GetService<IUserFeedService>().TryGetTarget(out userFeedService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            exceptionLogger.LogConfiguration = LogConfiguration;
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

                string message = await requestService.GetRequestBody(Request);

                await userFeedService.InsertUserPostAsync(token, message);

                return new Ok();
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
