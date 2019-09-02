using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;

using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.FactoriesManager;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Models;

using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;

using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;
using SentenceAPI.ActionResults;

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
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        private IUserServiceFactory userServiceFactory;
        private ITokenServiceFactory tokenServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        #region Services
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        #endregion

        #region Constructors
        public TokensController()
        {
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            exceptionLogger.LogConfiguration = LogConfiguration;
        }
        #endregion

        #region Controller's method
        /// <summary>
        /// If the user with given email, password exists then this method returns
        /// the jwt token for this user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(string email, string password)
        {
            try
            {
                password = password.GetMD5Hash();

                UserInfo user = await userService.Get(email, password);

                if (user == null)
                {
                    return Unauthorized();
                }

                var (encodedToken, securityToken) = tokenService.CreateEncodedToken(user);

                await tokenService.InsertTokenInDB(new JwtToken(securityToken, user));
                return new Ok(encodedToken);
            }
            catch (DatabaseException ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError();
            }
        }
        #endregion
    }
}
