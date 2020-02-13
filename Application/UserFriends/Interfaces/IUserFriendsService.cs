using MongoDB.Bson;

using System.Collections.Generic;
using System.Threading.Tasks;

using Domain.KernelInterfaces;
using Domain.UserFriends;


namespace SentenceAPI.Features.UserFriends.Interfaces
{
    public interface IUserFriendsService : IService
    {
        Task AddSubscriberAsync(string token, ObjectId subscriberID);
        Task AddSubscriptionAsync(string token, ObjectId subscriptionID);
        Task DeleteSubscriberAsync(string token, ObjectId subscriberID);
        Task DeleteSubscriptionAsync(string token, ObjectId subscriptionID);
        Task CreateUserFriendsRecord(ObjectId userID);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(string token);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(string token);

        Task<IEnumerable<Subscriber>> GetSubscribersAsync(ObjectId userID);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync(ObjectId userID);
    }
}