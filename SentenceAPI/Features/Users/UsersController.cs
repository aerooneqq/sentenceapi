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
using SentenceAPI.Extensions;

using Newtonsoft.Json;
using SentenceAPI.Features.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using SentenceAPI.ActionResults;

namespace SentenceAPI.Features.Users
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class UsersController : ControllerBase
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
            exceptionLogger.LogConfiguration = LogConfiguration;
        }

        [HttpGet, Route("search/login")]
        public async Task<IActionResult> FindUsersWithLogin([FromQuery]string login)
        {
            try
            {
                return new OkJson<IEnumerable<UserSearchResult>>((await userService.FindUsersWithLogin(login))
                    .Select(user => new UserSearchResult(user)));
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                string token = authorization.Split()[1];

                return new OkJson<UserInfo>(await userService.Get(token));
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

        /// <summary>
        /// Gets the user object and returns only data which was requied in the request.
        /// If the property which is marked with SECRET attribute will be listed in the list of properties,
        /// it will not be returned.
        /// </summary>
        /// <param name="properties">The list of properties devided by ',' or ';'</param>
        [HttpGet, Route("partial")]
        public async Task<IActionResult> Get([FromQuery]string properties)
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                string token = authorization.Split()[1];

                return new OkJson<Dictionary<string, object>>((await userService.Get(token))
                    .ConfigureNewObject(properties.Split(',', ';', StringSplitOptions.RemoveEmptyEntries)));
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

        public async Task<IActionResult> Get([FromQuery]long id)
        {
            try
            {
                return new OkJson<UserInfo>(await userService.Get(id));
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

                return new OkJson<UserInfo>(await userService.Get(user.ID));
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
