using Domain.KernelInterfaces;

namespace Domain.Date
{
    public interface IDateServiceFactory : IServiceFactory
    {
        IDateService GetService();
    }
}