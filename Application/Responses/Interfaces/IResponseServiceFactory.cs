using Domain.KernelInterfaces;

namespace Application.Responses.Interfaces
{
    public interface IResponseServiceFactory : IServiceFactory
    {
        IResponseService GetService();
    }
}