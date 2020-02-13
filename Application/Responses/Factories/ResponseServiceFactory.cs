using Application.Responses.Interfaces;

namespace Application.Responses.Factories
{
    public class ResponseServiceFactory : IResponseServiceFactory
    {
        public IResponseService GetService()
        {
            return new ResponseService();
        }
    }
}