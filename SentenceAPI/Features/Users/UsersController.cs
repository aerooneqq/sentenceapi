using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Links.Interfaces;

using Newtonsoft.Json;

namespace SentenceAPI.Features.Users
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class UsersController : Controller
    {
        public static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = string.Empty,
            ServiceName = "UsersController"
        };

        #region Services
        private ILinkService linkService;
        private IEmailService emailService;
        private IUserService<UserInfo> userService;
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;

        private ILinkServiceFactoty linkServiceFactory;
        private IUserServiceFactory userServiceFactory;
        private ILoggerFactory loggerFactory;
        private IEmailServiceFactory emailServiceFactory;
        #endregion

        public UsersController()
        {
            userServiceFactory = factoriesManager[typeof(IUserServiceFactory)].Factory
                as IUserServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;
            emailServiceFactory = factoriesManager[typeof(IEmailServiceFactory)].Factory
                as IEmailServiceFactory;
            linkServiceFactory = factoriesManager[typeof(ILinkServiceFactoty)].Factory as
                ILinkServiceFactoty;

            emailService = emailServiceFactory.GetService();
            userService = userServiceFactory.GetService();
            linkService = linkServiceFactory.GetService();

            exceptionLogger = loggerFactory.GetExceptionLogger();
            exceptionLogger.LogConfiguration = new Loggers.Models.LogConfiguration()
            {
                ControllerName = this.GetType().Name,
                ServiceName = string.Empty,
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(string email, string password)
        {
            try
            {
                return Json(await userService.Get(email, password));
            }
            catch (DatabaseException ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                return Json(await userService.Get(id));
            }
            catch (DatabaseException ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Creates a new user record in the mongo database. If the record was successful,
        /// then the letter with a link to activate the account is sent to the user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNewUser(string email, string password)
        {
            try
            {
                long id = await userService.CreateNewUser(email, password);
                UserInfo user = await userService.Get(id);

                string link = await linkService.CreateVerificationLink(user);
                await emailService.SendConfirmationEmail(link, user);

                return Ok();
            }
            catch (DatabaseException ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
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
