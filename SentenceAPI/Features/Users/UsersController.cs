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

using Newtonsoft.Json;
using SentenceAPI.Features.Loggers.Interfaces;

namespace SentenceAPI.Features.Users
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class UsersController : Controller
    {
        #region Services
        private IUserService<UserInfo> userService;
        private ILogger logger;
        #endregion

        #region Factories
        private FactoriesManager.FactoriesManager factoriesManager = 
            FactoriesManager.FactoriesManager.Instance;
        private IUserServiceFactory userServiceFactory;
        private ILoggerFactory loggerFactory;
        #endregion

        public UsersController()
        {
            userServiceFactory = factoriesManager[typeof(IUserServiceFactory)].Factory as IUserServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            userService = userServiceFactory.GetService();
            logger = loggerFactory.GetLogger();
            logger.LogConfiguration = new Loggers.Models.LogConfiguration()
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
                logger.Log(ex);
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
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
                logger.Log(ex);
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(string email, string password)
        {
            try
            {
                await userService.CreateNewUser(email, password);
                return Ok();
            }
            catch (DatabaseException ex)
            {
                logger.Log(ex);
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                return StatusCode(500);
            }
        }
    }
}
