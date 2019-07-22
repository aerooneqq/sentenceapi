using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserActivity.Interfaces
{
    public interface IUserActivityServiceFactory : IFactory
    {
        IUserActivityService GetService();
    }
}
