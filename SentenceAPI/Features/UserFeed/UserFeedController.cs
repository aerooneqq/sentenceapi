using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserFeed.Interfaces;

namespace SentenceAPI.Features.UserFeed
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class UserFeedController : Controller
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserFeedService userFeedService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private IUserFeedServiceFactory userFeedServiceFactory;
        #endregion

        public UserFeedController()
        {
            userFeedServiceFactory = factoriesManager[typeof(IUserFeedServiceFactory)].Factory as IUserFeedServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            exceptionLogger = loggerFactory.GetExceptionLogger();
            userFeedService = userFeedServiceFactory.GetService();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFeed()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                return Ok(JsonConvert.SerializeObject(await userFeedService.GetUserFeed(token)));
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
        public async Task<IActionResult> InsertFeed()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                string message = null;
                using (StreamReader sr = new StreamReader(Request.Body))
                {
                    message = await sr.ReadToEndAsync();
                }

                await userFeedService.InsertUserPost(token, message);

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
    }
}
