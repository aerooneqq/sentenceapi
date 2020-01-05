using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestService
    {
        Task<T> GetRequestBodyAsync<T>(HttpRequest request);

        Task<string> GetRequestBodyAsync(HttpRequest request);

        string GetToken(HttpRequest request);
    }
}
