using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.UserFriends.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;

namespace SentenceAPI.Features.UserFriends
{
    [Authorize, ApiController, Route("api/[controller]")]
    public class UserFriendsController : Controller
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserFriendsService userFriendsService;
        private IRequestService requestService; 
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public UserFriendsController()
        {
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
        }

        [HttpGet, Route("subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscribers = await userFriendsService.GetSubscribersAsync(token);

                return new OkJson<IEnumerable<Models.Subscriber>>(subscribers);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpGet, Route("subscriptions")]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscriptions = await userFriendsService.GetSubscriptionsAsync(token);

                return new OkJson<IEnumerable<Models.Subscription>>(subscriptions);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpPut, Route("subscribers")]
        public async Task<IActionResult> AddSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscribers = await userFriendsService.GetSubscribersAsync(token);

                if (subscribers.Any(s => s.UserID == subscriberID))
                {
                    return new BadSendedRequest<string>("The subscriber has been already added");
                }

                await userFriendsService.AddSubscriberAsync(token, subscriberID).ConfigureAwait(false);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpPut, Route("subscriptions")]
        public async Task<IActionResult> AddSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                var subscriptions = await userFriendsService.GetSubscriptionsAsync(token).ConfigureAwait(false);

                if (subscriptions.Any(s => s.UserID == subscriptionID))
                {
                    return new BadSendedRequest<string>("The subscription has been already added");
                }

                await userFriendsService.AddSubscriptionAsync(token, subscriptionID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpDelete, Route("subscribers")]
        public async Task<IActionResult> DeleteSubscriber([FromQuery]long subscriberID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.DeleteSubscriberAsync(token, subscriberID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpDelete, Route("subscriptions")]
        public async Task<IActionResult> DeleteSubscription([FromQuery]long subscriptionID)
        {
            try
            {
                string token = requestService.GetToken(Request);

                await userFriendsService.DeleteSubscriptionAsync(token, subscriptionID);

                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}
