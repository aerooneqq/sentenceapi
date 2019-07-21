using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.Response.Interfaces;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Databases.MongoDB.Interfaces;

namespace SentenceAPI.Features.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        #region Factories
        private readonly IFactoriesManager factoryManager = FactoriesManager.FactoriesManager.Instance;
        private IUserServiceFactory userServiceFactory;
        private ITokenServiceFactory tokenServiceFactory;
        #endregion

        #region Services
        private IUserService<UserInfo> userService;
        private ITokenService tokenService;
        #endregion

        #region Constructors
        public TokensController()
        {
            userServiceFactory = factoryManager[typeof(IUserServiceFactory)].Factory
                as IUserServiceFactory;
            tokenServiceFactory = factoryManager[typeof(ITokenServiceFactory)].Factory
                as ITokenServiceFactory;

            userService = userServiceFactory.GetService();
            tokenService = tokenServiceFactory.GetService();
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
                UserInfo user = await userService.Get(email, password);

                if (user == null)
                {
                    return Unauthorized();
                }

                var (encodedToken, securityToken) = tokenService.CreateEncodedToken(user);

                await tokenService.InsertTokenInDB(new JwtToken(securityToken, user));
                return Ok(encodedToken);
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        #endregion
    }
}
