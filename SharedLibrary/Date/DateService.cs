using System;

using SharedLibrary.Date.Interfaces;


namespace SharedLibrary.Date
{
    public class DateService : IDateService
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.UtcNow;
        }
    }
}
