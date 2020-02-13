using System.Net;
using System.Threading.Tasks;
using Domain.KernelInterfaces;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace Application.Requests.Interfaces
{
    public interface IRequestService : IService
    {
        Task<string> Get(string url);
        Task<string> GetRequestBody(HttpRequest request);
        Task<T> GetRequestBody<T>(HttpRequest request);
        string GetToken(HttpRequest request);
        Task<ObjectId> LogRequestToDatabase(HttpRequest request);
        Task RedirectRequest(HttpContext context, string url);
    }
}