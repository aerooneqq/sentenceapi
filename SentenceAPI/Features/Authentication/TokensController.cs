using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;

using SharedLibrary.FactoriesManager.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SharedLibrary.FactoriesManager;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Models;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;
using SharedLibrary.ActionResults;
using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Features.UserActivity.Interfaces;

namespace SentenceAPI.Features.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        public static LogConfiguration LogConfiguration { get; } = new LogConfiguration()
        {
            ControllerName = "TokensController",
            ServiceName = string.Empty
        };

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        #region Services
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IUserActivityService userActivityService;
        #endregion

        #region Constructors
        public TokensController()
        {
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);

            exceptionLogger.LogConfiguration = LogConfiguration;
        }
        #endregion

        #region Controller's method
        /// <summary>
        /// If the user with given email, password exists then this method returns
        /// the jwt token for this user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string email, [FromQuery]string password)
        {
            try
            {
                if (email is null || password is null) 
                { 
                    return new BadSendedRequest<string>("Email and password must be defined");
                }
                
                password = password.GetMD5Hash();

                UserInfo user = await userService.GetAsync(email, password);

                if (user == null || user.IsAccountDeleted)
                {
                    return Unauthorized();
                }

                var (encodedToken, securityToken) = tokenService.CreateEncodedToken(user);

                await tokenService.InsertTokenInDBAsync(new JwtToken(securityToken, user)).ConfigureAwait(false);

                DefferedTasksManager.AddTask(new Action(() => userActivityService.AddSingleActivityAsync(user.ID,
                    new UserActivity.Models.SingleUserActivity()
                    {
                        ActivityDate = DateTime.Now,
                        Activity = "Logged in"
                    })));

                return new Ok(encodedToken);
            }
            catch (DatabaseException ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex));
                return new InternalServerError();
            }
        }
        #endregion
    }
}
