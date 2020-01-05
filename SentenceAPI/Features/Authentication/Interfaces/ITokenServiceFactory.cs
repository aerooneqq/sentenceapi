﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DatabasesManager.Interfaces;
using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.Authentication.Interfaces
{
    public interface ITokenServiceFactory : IServiceFactory
    {
        ITokenService GetService(IDatabaseManager databaseManager);
    }
}
