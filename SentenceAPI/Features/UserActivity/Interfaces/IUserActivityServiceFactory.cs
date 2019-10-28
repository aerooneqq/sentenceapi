using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserActivity.Interfaces
{
    public interface IUserActivityServiceFactory : IServiceFactory
    {
        IUserActivityService GetService();
    }
}
