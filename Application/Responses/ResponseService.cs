using System.Net;
using System.Threading.Tasks;
using Application.Responses.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Responses
{
    public class ResponseService : IResponseService
    {
        public IResponseCopier ResponseCopier { get; }
        
        
        public ResponseService()
        {
            ResponseCopier = new ResponseCopier();
        }
    }
}