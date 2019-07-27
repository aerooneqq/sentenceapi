using SentenceAPI.Features.UserFriends.Models;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.KernelInterfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsService : IService
    {
        Task AddSubscriber(long userID, long subscriberID);
        Task AddSubscription(long userID, long subscriptionID);
        Task DeleteSubscriber(long userID, long subscriberID);
        Task DeleteSubscription(long userID, long subscriptionID);
        Task<IEnumerable<Subscriber>> GetSubscribers(string token);
        Task<IEnumerable<Subscription>> GetSubscriptions(string token);
    }
}
