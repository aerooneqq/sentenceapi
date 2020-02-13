using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using System;
using System.Threading.Tasks;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;

using Application.Codes.Interfaces;
using Application.Requests.Interfaces;
using Application.Users.Interfaces;


namespace SentenceAPI.Features.Codes
{
    [Authorize, ApiController, Route("sentenceapi/[controller]")]
    public class CodesController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IUserService<UserInfo> userService;
        private readonly ICodesService codesService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public CodesController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            logConfiguration = new LogConfiguration(this.GetType());
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}
