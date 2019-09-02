using SentenceAPI.Features.Requests.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Requests.Factories
{
    class RequestServiceFactory : IRequestServiceFactory
    {
        public IRequestService GetService()
        {
            return new RequestService();
        }
    }
}
