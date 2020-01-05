using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.ApplicationFeatures.Requests.Factories
{
    class RequestServiceFactory : IRequestServiceFactory
    {
        public IRequestService GetService()
        {
            return new RequestService();
        }
    }
}
