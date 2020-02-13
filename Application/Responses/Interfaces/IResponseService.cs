using System.Net;
using System.Threading.Tasks;

using Domain.KernelInterfaces;

using Microsoft.AspNetCore.Http;


namespace Application.Responses.Interfaces
{
    public interface IResponseService : IService
    {
        IResponseCopier ResponseCopier { get; }
    }
}