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
using SentenceAPI.Features.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;

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

        private ILinkServiceFactory linkServiceFactory;
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
            linkServiceFactory = factoriesManager[typeof(ILinkServiceFactory)].Factory as
                ILinkServiceFactory;

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

        [HttpGet, Route("search/login")]
        public async Task<IActionResult> FindUsersWithLogin([FromQuery]string login)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject((await userService.FindUsersWithLogin(login)).Select(u =>
                {
                    return new
                    {
                        userID = u.ID,
                        name = u.Name + u.Surname,
                        birthDate = u.BirthDate,
                    };
                })));
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                string token = authorization.Split()[1];

                return Json(await userService.Get(token));
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Get([FromQuery]string email, [FromQuery]string password)
        {
            try
            {
                return Json(await userService.Get(email, password));
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Get([FromQuery]long id)
        {
            try
            {
                return Json(await userService.Get(id));
            }
            catch (DatabaseException ex)
            {
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
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser()
        {
            try
            {
                UserInfo user = null;
                using (StreamReader sr = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string body = await sr.ReadToEndAsync();
                    user = JsonConvert.DeserializeObject<UserInfo>(body);
                }

                await userService.Update(user);

                return Ok(JsonConvert.SerializeObject(await userService.Get(user.ID)));
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex.Message));
                return StatusCode(500);
            }
        }
    }
}
