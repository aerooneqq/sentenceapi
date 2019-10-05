using SentenceAPI.ApplicationFeatures.Date.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Date
{
    public class DateService : IDateService
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.UtcNow;
        }
    }
}
