using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using Application.Users.Interfaces;
using Application.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Events;

using SentenceAPI.Extensions;
using SentenceAPI.Features.Users.Events;

using DataAccessLayer.Exceptions;

using Domain.Extensions;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;
using Domain.Validators;

using MongoDB.Bson;


namespace SentenceAPI.Features.Users
{
    [Route("sentenceapi/[controller]"), ApiController]
    public class UsersController : ControllerBase
    {
        #region Services
        private readonly IFactoriesManager factoriesManager;

        private readonly IUserService<UserInfo> userService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public UsersController(IMemoryCache memoryCacheService, IFactoriesManager factoriesManager)
        { 
            this.factoriesManager = factoriesManager;

            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(this.GetType());
        }

        [HttpGet, Route("search/login"), Authorize]
        public async Task<IActionResult> FindUsersWithLogin([FromQuery]string login)
        {
            try
            {
                var userInfoResult = await userService.FindUsersWithLoginAsync(login)
                    .ConfigureAwait(false);
                var userSearchResult = userInfoResult.Select(user => new UserSearchResult(user));

                return new OkJson<IEnumerable<UserSearchResult>>(userSearchResult);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
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

                var user = await userService.GetAsync(token).ConfigureAwait(false);

                return new OkJson<Dictionary<string, object>>(user.ConfigureNewObject(properties.Split(new char[] { ',', ';'})));
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

        [Authorize]
        public async Task<IActionResult> Get([FromQuery]ObjectId id)
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
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
                    return new BadSentRequest<IEnumerable<string>>(errors);
                }

                if (await userService.DoesUserExistAsync(email).ConfigureAwait(false))
                    return new NoContent();
                
                ObjectId id = await userService.CreateNewUserAsync(email, password).ConfigureAwait(false);
                var user = await userService.GetAsync(id).ConfigureAwait(false);
                    
                await EventManager.Raise(new UserCreatedEvent(factoriesManager, user)).ConfigureAwait(false);

                return new Created();

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
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                UserInfo user = new UserInfo(updatedFields)
                {
                    ID = userID
                };

                if (updatedFields.Keys.Contains("email"))
                {
                    user.IsAccountVerified = false;
                    updatedFields.Add(typeof(UserInfo).GetBsonPropertyName("IsAccountVerified"), false);

                    await EventManager.Raise(new UserEmailChangedEvent(user.Email, user.ID, factoriesManager)).
                        ConfigureAwait(false);
                }

                await userService.UpdateAsync(user, updatedFields.Keys.Select(propName => 
                    typeof(UserInfo).GetPropertyFromBSONName(propName).Name));

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

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                UserInfo user = await userService.GetAsync(requestService.GetToken(Request)).ConfigureAwait(false);
                user.IsAccountDeleted = true;

                await userService.UpdateAsync(user, new[] { "IsAccountDeleted" }).ConfigureAwait(false);

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
