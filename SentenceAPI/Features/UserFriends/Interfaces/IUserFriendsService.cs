using MongoDB.Bson;
using SentenceAPI.Features.UserFriends.Models;
using SentenceAPI.Features.Users.Models;

using SharedLibrary.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsService : IService
    {
        Task AddSubscriberAsync(string token, ObjectId subscriberID);
        Task AddSubscriptionAsync(string token, ObjectId subscriptionID);
        Task DeleteSubscriberAsync(string token, ObjectId subscriberID);
        Task DeleteSubscriptionAsync(string token, ObjectId subscriptionID);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(string token);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(string token);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(ObjectId userID);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(ObjectId userID);
    }
}