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

namespace SentenceAPI.Features.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        #region Factories
        private IFactoriesManager factoryManager = FactoriesManager.FactoriesManager.Instance;
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
            userServiceFactory = factoryManager[typeof(IUserService<UserInfo>)].Factory
                as IUserServiceFactory;
            tokenServiceFactory = factoryManager[typeof(ITokenService)].Factory as ITokenServiceFactory;

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
        public async Task<string> Get(string email, string password)
        {
            UserInfo user = userService.Get(email, password);

            if (user == null)
            {
                Response.StatusCode = 401;
                return "";
            }

            string encodedToken = tokenService.CreateEncodedToken(user);

            Response.StatusCode = 200;
            Response.ContentType = "application/json";

            return encodedToken;
        }
        #endregion
    }
}
