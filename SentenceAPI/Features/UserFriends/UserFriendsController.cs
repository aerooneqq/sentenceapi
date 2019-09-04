﻿using DataAccessLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentenceAPI.ActionResults;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
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
        private static LogConfiguration LogConfiguration => new LogConfiguration()
        {
            ControllerName = "UserFriendsController",
            ServiceName = string.Empty,
        };

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserFriendsService userFriendsService;
        private IRequestService requestService; 
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public UserFriendsController()
        {
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            
            exceptionLogger.LogConfiguration = LogConfiguration;
        }

        [HttpGet, Route("subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscribers = await userFriendsService.GetSubscribers(token);

                return new OkJson<IEnumerable<Models.Subscriber>>(subscribers);
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

        [HttpGet, Route("subscriptions")]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscriptions = await userFriendsService.GetSubscriptions(token);

                return new OkJson<IEnumerable<Models.Subscription>>(subscriptions);
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

        [HttpPut, Route("subscribers")]
        public async Task<IActionResult> AddSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.AddSubscriber(token, subscriberID);

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

        [HttpPut, Route("subscriptions")]
        public async Task<IActionResult> AddSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.AddSubscription(token, subscriptionID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError();
            }
        }

        [HttpDelete, Route("subscribers")]
        public async Task<IActionResult> DeleteSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.DeleteSubscriber(token, subscriberID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError();
            }
        }

        [HttpDelete, Route("subscriptions")]
        public async Task<IActionResult> DeleteSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.DeleteSubscription(token, subscriptionID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                await exceptionLogger.Log(new ApplicationError(ex.Message));
                return new InternalServerError();
            }
        }
    }
}
