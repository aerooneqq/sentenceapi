using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.KernelInterfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Word.Interfaces
{
    public interface IWordServiceFactory : IServiceFactory
    {
        IWordService GetService(IFactoriesManager factoriesManager);
    }
}