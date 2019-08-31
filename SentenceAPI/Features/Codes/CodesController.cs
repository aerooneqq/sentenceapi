using DataAccessLayer.DatabasesManager;
using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.ActionResults;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        public CodesController()
        {
            loggerFactory = (ILoggerFactory)factoriesManager[typeof(ILoggerFactory)];
            codesServiceFactory = (ICodesServiceFactory)factoriesManager[typeof(ICodesServiceFactory)];
            userServiceFactory = (IUserServiceFactory)factoriesManager[typeof(IUserServiceFactory)];

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = logConfiguration;

            codesService = codesServiceFactory.GetService();
            userService = userServiceFactory.GetService();
        }

        [HttpPut]
        public async Task<IActionResult> ActivateAccount()
        {
            try
            {
                await codesService.ActivateCode(GetRequestBody(Request)).ConfigureAwait(false);
                UserInfo user = await userService.Get(GetToken(Request)).ConfigureAwait(false);

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

        private string GetRequestBody(HttpRequest request)
        {
            using (StreamReader sr = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                return sr.ReadToEnd();
            }
        }

        private string GetToken(HttpRequest request)
        {
            string authHeader = request.Headers["Authorization"];
            return authHeader.Split()[1];
        }
    }
}
