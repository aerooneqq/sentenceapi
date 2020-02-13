using Domain.KernelInterfaces;

namespace Application.Hash.Interfaces
{
    public interface IHashService : IService
    {
        string GetHash(byte[] bytes);
    }
}