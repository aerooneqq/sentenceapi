using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SentenceAPI.Features.Email
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class EmailController
    {
    }
}
