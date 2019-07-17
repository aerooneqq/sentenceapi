using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Response.Interfaces
{
    public interface IResponseService
    {
        void ConfigureResponse(HttpResponse response);
    }
}
