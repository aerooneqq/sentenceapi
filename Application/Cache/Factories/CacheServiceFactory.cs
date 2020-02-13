using Application.Caching.Interfaces;

namespace Application.Caching.Factories
{
    public class CacheServiceFactory : ICacheServiceFactory
    {
        public ICacheService GetService()
        {
            return new CacheService();
        }
    }
}