using Application.Hash.Interfaces;

namespace Application.Hash.Factories
{
    public class HashServiceFactory : IHashServiceFactory
    {
        public IHashService GetService()
        {
            return new HashService();
        }
    }
}