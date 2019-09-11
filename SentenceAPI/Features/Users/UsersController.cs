using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Validators;
using SentenceAPI.Extensions;

using Newtonsoft.Json;

using SentenceAPI.ActionResults;

using DataAccessLayer.Exceptions;
using SentenceAPI.KernelInterfaces;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.Features.Codes.Interfaces;
using Microsoft.AspNetCore.Http;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Features.Authentication.Interfaces;

namespace SentenceAPI.Features.Users
{
    [Route("api/[controller]"), ApiController]
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
        private ICodesService codesService;
        private IRequestService requestService;
        private IMemoryCache memoryCacheService;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;
        #endregion

        public UsersController(IMemoryCache memoryCacheService)
        {
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ILinkService>().TryGetTarget(out linkService);
            factoriesManager.GetService<IEmailService>().TryGetTarget(out emailService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            this.memoryCacheService = memoryCacheService;

            exceptionLogger.LogConfiguration = LogConfiguration;
        }

        [HttpGet, Route("search/login"), Authorize]
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

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                string token = authorization.Split()[1];

                var user = await userService.Get(token);

                return new OkJson<UserInfo>(user);
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
        [HttpGet, Route("partial"), Authorize]
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

        [Authorize]
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
        public async Task<IActionResult> CreateNewUser([FromQuery]string email, [FromQuery]string password)
        {
            try
            {
                (bool validationResult, IEnumerable<string> errors) = ValidateEmailAndPassword(email, password);

                if (!validationResult)
                {
                    return new BadSendedRequest<IEnumerable<string>>(errors);
                }

                if (!(await userService.DoesUserExist(email).ConfigureAwait(false)))
                {
                    long id = await userService.CreateNewUser(email, password);

                    ActivationCode activationCode = codesService.CreateActivationCode(id);
                    await codesService.InsertCodeInDatabase(activationCode).ConfigureAwait(false);

                    await emailService.SendConfirmationEmail(activationCode.Code, email);

                    return new Created();
                }

                return new NoContent();
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

        private (bool, IEnumerable<string>) ValidateEmailAndPassword(string email, string password)
        {
            (bool emailValidation, string emailError) = new EmailValidator(email).Validate();
            (bool passValidation, string passError) = new PasswordValidator(password).Validate();

            if (emailValidation && passValidation)
            {
                return (true, new string[] { });
            }

            return (false, new[] { emailError, passError });
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateUser()
        {
            try
            {
                var updatedFields = await requestService.GetRequestBody<Dictionary<string, object>>(Request);
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                UserInfo user = new UserInfo(updatedFields);
                user.ID = userID;

                if (updatedFields.Keys.Contains("email"))
                {
                    user.IsAccountVerified = false;

                    var activationCode = codesService.CreateActivationCode(user.ID);
                    await codesService.InsertCodeInDatabase(activationCode).ConfigureAwait(false);

                    await emailService.SendConfirmationEmail(activationCode.Code, user.Email).ConfigureAwait(false);
                }

                await userService.Update(user, updatedFields.Keys.Select(propName =>
                {
                    return typeof(UserInfo).GetPropertyFromJsonName(propName).Name;
                }));

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
