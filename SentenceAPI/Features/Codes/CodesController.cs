using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.ActionResults;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;

namespace SentenceAPI.Features.Codes
{
    [Authorize, ApiController, Route("api/[controller]")]
    public class CodesController : Controller
    {
        #region Static fields
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = "CodesController",
            ServiceName = string.Empty
        };
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private ICodesServiceFactory codesServiceFactory;
        private IUserServiceFactory userServiceFactory;
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserService<UserInfo> userService;
        private ICodesService codesService;
        private IRequestService requestService;
        #endregion

        public CodesController()
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        [HttpPut]
        public async Task<IActionResult> ActivateAccount()
        {
            try
            {
                await codesService.ActivateCode(await requestService.GetRequestBody(Request)
                    .ConfigureAwait(false)).ConfigureAwait(false);
                UserInfo user = await userService.Get(requestService.GetToken(Request)).ConfigureAwait(false);

                user.IsAccountVerified = true;
                await userService.Update(user).ConfigureAwait(false);

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
