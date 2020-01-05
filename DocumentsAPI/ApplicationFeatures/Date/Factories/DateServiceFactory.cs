using SharedLibrary.Date;
using SharedLibrary.Date.Interfaces;


namespace DocumentsAPI.ApplicationFeatures.Date.Factories
{
    public class DateServiceFactory : IDateServiceFactory
    {
        public IDateService GetService()
        {
            return new DateService();
        }
    }
}