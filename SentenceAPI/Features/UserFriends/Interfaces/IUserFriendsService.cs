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
        Task AddSubscriber(string token, long subscriberID);
        Task AddSubscription(string token, long subscriptionID);
        Task DeleteSubscriber(string token, long subscriberID);
        Task DeleteSubscription(string token, long subscriptionID);

        Task<IEnumerable<Subscriber>> GetSubscribers(string token);
        Task<IEnumerable<Subscription>> GetSubscriptions(string token);

        Task<IEnumerable<Subscriber>> GetSubscribers(long userID);
        Task<IEnumerable<Subscription>> GetSubscriptions(long userID);
    }
}
