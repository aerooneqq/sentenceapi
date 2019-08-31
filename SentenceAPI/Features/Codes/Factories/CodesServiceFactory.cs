using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Codes.Factories
{
    class CodesServiceFactory : ICodesServiceFactory
    {
        public ICodesService GetService()
        {
            return new CodesService();
        }
    }
}
