namespace Domain.Date
{
    public class DateServiceFactory : IDateServiceFactory
    {
        public IDateService GetService()
        {
            return new DateService();
        }
    }
}