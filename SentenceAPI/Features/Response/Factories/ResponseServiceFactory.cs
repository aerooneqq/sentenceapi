using SentenceAPI.Features.Response.Interfaces;
using SentenceAPI.Features.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Response.Factories
{
    public class ResponseServiceFactory : IResponseServiceFactory
    {
        public IResponseService GetService()
        {
            return new ResponseService();
        }
    }
}
