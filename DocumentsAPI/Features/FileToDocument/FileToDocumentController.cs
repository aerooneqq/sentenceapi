using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;
using DocumentsAPI.Features.FileToDocument.Interfaces;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;


namespace DocumentsAPI.Features.FileToDocument
{
    [Route("api/[controller]"), ApiController]
    public class FileToDocumentController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFileToDocumentService fileToDocumentService;
        #endregion


        public FileToDocumentController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IFileToDocumentService>().TryGetTarget(out fileToDocumentService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(GetType())
            {
                ClassName = this.GetType().FullName,
                ComponentType = ComponentType.Controller
            };
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentIDFromFileID(long fileID)
        {
            try
            {
                var documentID = await fileToDocumentService.GetDocumentIDAsync(fileID).ConfigureAwait(false);
                return new OkJson<long>(documentID);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IActionResult> AssociateFileWithDocument(long fileID, long documentID)
        {
            try 
            {
                await fileToDocumentService.AssociateFileWithDocumentAsync(fileID, documentID).ConfigureAwait(false);
                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}