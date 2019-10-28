using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.ActionResults.ResponseBuilder
{
    internal interface IResponseBuilder
    {
        IResponseBuilder SetStatusCode(int statusCode);
        IResponseBuilder SetContentType(string contentType);
        IResponseBuilder SetContentLength(int contentLength);
        IResponseBuilder SetContentLength();
        IResponseBuilder SetContent(string content);
        IResponseBuilder SetCORSHeaders();

        HttpResponse Build();
    }
}
