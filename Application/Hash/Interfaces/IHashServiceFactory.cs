using Domain.KernelInterfaces;

namespace Application.Hash.Interfaces
{
    public interface IHashServiceFactory : IServiceFactory
    {
        IHashService GetService();
    }
}