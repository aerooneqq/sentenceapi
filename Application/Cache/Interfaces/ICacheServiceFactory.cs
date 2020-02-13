namespace Application.Caching.Interfaces
{
    public interface ICacheServiceFactory
    {
        ICacheService GetService();
    }
}