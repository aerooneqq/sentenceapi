using Application.Word.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace Application.Word.Factories
{
    public class WordServiceFactory : IWordServiceFactory
    {
        public IWordService GetService(IFactoriesManager factoriesManager)
        {
            return new WordService(factoriesManager);
        }
    }
}