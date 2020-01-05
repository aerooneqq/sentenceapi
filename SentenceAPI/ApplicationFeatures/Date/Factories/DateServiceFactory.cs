using SharedLibrary.Date.Interfaces;
using SharedLibrary.Date;


namespace SentenceAPI.ApplicationFeatures.Date.Factories
{
    public class DateServiceFactory : IDateServiceFactory
    {
        public IDateService GetService()
        {
            return new DateService();
        }
    }
}
