using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using DocumentsAPI.Features.Documents.Interfaces;
using DocumentsAPI.Features.FileToDocument.Interfaces;
using DocumentsAPI.Models.Document;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DocumentsAPI.Features.FileToDocument
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class FileToDocumentController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFileToDocumentService fileToDocumentService;
        private readonly IDocumentService documentService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public FileToDocumentController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IFileToDocumentService>().TryGetTarget(out fileToDocumentService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            logConfiguration = new LogConfiguration(GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentIDFromFileID([FromQuery]ObjectId fileID)
        {
            try
            {
                var documentID = await fileToDocumentService.GetDocumentIDAsync(fileID).ConfigureAwait(false);
                return new OkJson<ObjectId>(documentID);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IActionResult> AssociateFileWithDocument(
            [FromQuery]ObjectId fileID, [FromQuery]ObjectId userID,
            [FromQuery]string fileName, [FromQuery]DocumentType documentType
            )
        {
            try 
            {
                var fileToDocument = await fileToDocumentService.GetFileToDocumentModelAsync(fileID).ConfigureAwait(false);

                if (fileToDocument is {} && fileToDocument.FileID == fileID)
                {
                    return new BadSendedRequest<string>("File already associated with the document");
                }

                ObjectId documentID = await documentService.CreateEmptyDocument(userID, fileName, documentType);
                await fileToDocumentService.AssociateFileWithDocumentAsync(fileID, documentID).ConfigureAwait(false);
                return new Ok();
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}