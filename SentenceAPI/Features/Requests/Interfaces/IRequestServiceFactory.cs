﻿using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Requests.Interfaces
{
    interface IRequestServiceFactory : IServiceFactory
    {
        IRequestService GetService();
    }
}
