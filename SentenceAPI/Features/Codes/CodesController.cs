using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;

using SentenceAPI.Features.Codes.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SharedLibrary.Loggers.Configuration;

using System;
using System.Threading.Tasks;


namespace SentenceAPI.Features.Codes
{
    [Authorize, ApiController, Route("api/[controller]")]
    public class CodesController : Controller
    {
        #region Factories
        private IFactoriesManager factoriesManager = ManagersDictionary.Instance.GetManager(Startup.ApiName);
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
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
        }

        [HttpPut]
        public async Task<IActionResult> ActivateAccount()
        {
            try
            {
                await codesService.ActivateCodeAsync(await requestService.GetRequestBody(Request)
                    .ConfigureAwait(false)).ConfigureAwait(false);
                UserInfo user = await userService.GetAsync(requestService.GetToken(Request)).ConfigureAwait(false);

                user.IsAccountVerified = true;
                await userService.UpdateAsync(user).ConfigureAwait(false);

                return new Ok();
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
