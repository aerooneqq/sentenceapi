using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.UserFriends.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using MongoDB.Bson;

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


        public UserFriendsController(IFactoriesManager factoriesManager)
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
        public async Task<IActionResult> AddSubscriber([FromQuery]ObjectId subscriberID)
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
        public async Task<IActionResult> AddSubscription([FromQuery]ObjectId subscriptionID)
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
        public async Task<IActionResult> DeleteSubscriber([FromQuery]ObjectId subscriberID)
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
        public async Task<IActionResult> DeleteSubscription([FromQuery]ObjectId subscriptionID)
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
