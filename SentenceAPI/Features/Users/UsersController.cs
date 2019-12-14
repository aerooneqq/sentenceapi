using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Validators;
using SentenceAPI.Extensions;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Events;
using SentenceAPI.Features.Users.Events;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using DataAccessLayer.Exceptions;

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
        private readonly ILinkService linkService;
        private readonly IEmailService emailService;
        private readonly IUserService<UserInfo> userService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ICodesService codesService;
        private readonly IRequestService requestService;
        private readonly IMemoryCache memoryCacheService;
        private readonly ITokenService tokenService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager =
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
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
                var userInfoResult = await userService.FindUsersWithLoginAsync(login).ConfigureAwait(false);
                var userSearchResult = userInfoResult.Select(user => new UserSearchResult(user));

                return new OkJson<IEnumerable<UserSearchResult>>(userSearchResult);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var user = await userService.GetAsync(requestService.GetToken(Request)).ConfigureAwait(false);

                return new OkJson<UserInfo>(user);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
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
                string token = requestService.GetToken(Request);

                return new OkJson<Dictionary<string, object>>((await userService.GetAsync(token).ConfigureAwait(false))
                    .ConfigureNewObject(properties.Split(',', ';', StringSplitOptions.RemoveEmptyEntries)));
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }

        [Authorize]
        public async Task<IActionResult> Get([FromQuery]long id)
        {
            try
            {
                return new OkJson<UserInfo>(await userService.GetAsync(id).ConfigureAwait(false));
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
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

                if (!(await userService.DoesUserExistAsync(email).ConfigureAwait(false)))
                {
                    long id = await userService.CreateNewUserAsync(email, password).ConfigureAwait(false);

                    ActivationCode activationCode = codesService.CreateActivationCode(id);
                    await codesService.InsertCodeInDatabaseAsync(activationCode).ConfigureAwait(false);

                    await emailService.SendConfirmationEmailAsync(activationCode.Code, email).ConfigureAwait(false);

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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
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
                var updatedFields = await requestService.GetRequestBody<Dictionary<string, object>>(Request)
                    .ConfigureAwait(false);
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                UserInfo user = new UserInfo(updatedFields)
                {
                    ID = userID
                };

                if (updatedFields.Keys.Contains("email"))
                {
                    user.IsAccountVerified = false;
                    updatedFields.Add(typeof(UserInfo).GetBsonPropertyName("IsAccountVerified"), false);

                    await EventManager.Raise(new UserEmailChangedEvent(user.Email, user.ID));
                }

                await userService.UpdateAsync(user, updatedFields.Keys.Select(propName =>
                {
                    return typeof(UserInfo).GetPropertyFromBSONName(propName).Name;
                }));

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                UserInfo user = await userService.GetAsync(requestService.GetToken(Request)).ConfigureAwait(false);
                user.IsAccountDeleted = true;

                await userService.UpdateAsync(user, new[] { "IsAccountDeleted" })
                    .ConfigureAwait(false);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }
    }
}
