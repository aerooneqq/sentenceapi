using Microsoft.AspNetCore.Mvc;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;

namespace DocumentsAPI.Features.FileToDocument
{
    [Route("api/[controller]"), ApiController]
    public class FileToDocumentController : Controller
    {
        #region Factories
        private IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(DocumentsAPI.Startup.ApiName);
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion
    }
}