using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace DocumentsAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService();
    }
}
