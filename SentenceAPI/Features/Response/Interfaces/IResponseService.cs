using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Response.Interfaces
{
    public interface IResponseService
    {
        IResponseService SetStatus(int status);
        IResponseService SetContentType(string contentType);
        IResponseService SetContentLength(int contentLength);
        IResponseService SetContent(string content);

        void ConfigureResponse(HttpResponse response);
    }
}
