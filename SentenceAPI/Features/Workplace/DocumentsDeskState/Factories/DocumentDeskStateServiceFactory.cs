﻿using DataAccessLayer.DatabasesManager.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Services;
using SharedLibrary.FactoriesManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState.Factories
{
    public class DocumentDeskStateServiceFactory : IDocumentDeskStateServiceFactory
    {
        public IDocumentDeskStateService GetService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            return new DocumentDeskStateService(factoriesManager, databaseManager);
        }
    }
}
