using Microsoft.AspNetCore.Http;

using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DocumentsAPI.ApplicationFeatures.Requests.Interfaces
{
    internal interface IRequestService
    {
        Task<T> GetRequestBodyAsync<T>(HttpRequest request);

        Task<string> GetRequestBodyAsync(HttpRequest request);

        string GetToken(HttpRequest request);
        
        Task<bool> CheckIfRequestInDatabase(ObjectId requestID);
     
        ObjectId GetUserID(HttpRequest request);
    }
}
