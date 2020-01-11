using DocumentsAPI.Features.Documents.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;

namespace DocumentsAPI.Features.Documents
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class DocumentsController
    {
        #region Services
        private readonly IDocumentService documentService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public DocumentsController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());
        }
    }
}