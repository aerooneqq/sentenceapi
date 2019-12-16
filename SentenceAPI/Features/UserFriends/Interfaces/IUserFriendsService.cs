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
        Task AddSubscriberAsync(string token, long subscriberID);
        Task AddSubscriptionAsync(string token, long subscriptionID);
        Task DeleteSubscriberAsync(string token, long subscriberID);
        Task DeleteSubscriptionAsync(string token, long subscriptionID);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(string token);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(string token);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(long userID);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(long userID);
    }
}