using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentenceAPI.Databases.Exceptions;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Features.Loggers.Models;
using SentenceAPI.Features.UserFriends.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends
{
    [Authorize, ApiController, Route("api/[controller]")]
    public class UserFriendsController : Controller
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger; 
        private IUserFriendsService userFriendsService;
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

        private ILoggerFactory loggerFactory;
        private IUserFriendsServiceFactory userFriendsServiceFactory;
        #endregion

        #region Builders
        private IMongoDBServiceBuilder<Models.UserFriends> mongoDBServiceBuilder;
        #endregion

        public UserFriendsController()
        {
            userFriendsServiceFactory = factoriesManager[typeof(IUserFriendsServiceFactory)].Factory 
                as IUserFriendsServiceFactory;
            loggerFactory = factoriesManager[typeof(ILoggerFactory)].Factory as ILoggerFactory;

            exceptionLogger = loggerFactory.GetExceptionLogger();
            userFriendsService = userFriendsServiceFactory.GetSerivce();
        }

        [HttpGet, Route("subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                return Ok(await userFriendsService.GetSubscribers(token));
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

        [HttpGet, Route("subscriptions")]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                return Ok(JsonConvert.SerializeObject(await userFriendsService.GetSubscriptions(token)));
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

        [HttpPut, Route("subscribers")]
        public async Task<IActionResult> AddSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                await userFriendsService.AddSubscriber(token, subscriberID);

                return Ok();
            }
            catch (DatabaseException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut, Route("subscriptions")]
        public async Task<IActionResult> AddSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                await userFriendsService.AddSubscription(token, subscriptionID);

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

        [HttpDelete, Route("subscribers")]
        public async Task<IActionResult> DeleteSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                await userFriendsService.DeleteSubscriber(token, subscriberID);

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

        [HttpDelete, Route("subscriptions")]
        public async Task<IActionResult> DeleteSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                string token = authHeader.Split()[1];

                await userFriendsService.DeleteSubscription(token, subscriptionID);

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
