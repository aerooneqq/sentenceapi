using Domain.KernelInterfaces;

namespace Application.Caching.Interfaces
{
    public interface ICacheServiceFactory : IServiceFactory
    {
        ICacheService GetService();
    }
}