using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestService
    {
        T GetRequestBody<T>(HttpRequest request);

        string GetRequestBody(HttpRequest request);

        string GetToken(HttpRequest request);
    }
}
