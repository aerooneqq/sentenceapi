using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using SentenceAPI.Features.Users.Interfaces;

namespace SentenceAPI.Features.Authentication
{
    [Route("/token")]
    public class TokensController : ApiController
    {
        

        public HttpResponseMessage Get()
        {

        }
    }
}
