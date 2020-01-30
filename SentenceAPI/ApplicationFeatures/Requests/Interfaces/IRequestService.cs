using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;


namespace SentenceAPI.ApplicationFeatures.Requests.Interfaces
{
    interface IRequestService
    {
        Task<T> GetRequestBody<T>(HttpRequest request);

        Task<string> GetRequestBody(HttpRequest request);

        string GetToken(HttpRequest request);

        Task<ObjectId> LogRequestToDatabase(HttpRequest request);
    }
}
