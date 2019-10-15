using SentenceAPI.ApplicationFeatures.Date.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
