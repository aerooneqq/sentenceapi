using System;
using System.Threading.Tasks;
using Application.Documents.Documents.Interfaces;
using Application.Documents.FileToDocument.Interfaces;
using DataAccessLayer.Exceptions;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Models.Document;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using Microsoft.AspNetCore.Mvc;


namespace DocumentsAPI.Features.FileToDocument
{
    [Route("documentsapi/[controller]"), ApiController]
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
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            
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
            [FromQuery]string fileID, [FromQuery]string userID,
            [FromQuery]string fileName, [FromQuery]DocumentType documentType
            )
        {
            try 
            {
                ObjectId fileObjectId = ObjectId.Parse(fileID);
                ObjectId userObjectId = ObjectId.Parse(userID);
                var fileToDocument = await fileToDocumentService.GetFileToDocumentModelAsync(fileObjectId).
                    ConfigureAwait(false);

                if (fileToDocument is {} && fileToDocument.FileID == fileObjectId)
                {
                    return new BadSentRequest<string>("File already associated with the document");
                }

                ObjectId documentID = await documentService.CreateEmptyDocument(userObjectId, fileName, documentType);
                await fileToDocumentService.AssociateFileWithDocumentAsync(fileObjectId, documentID).ConfigureAwait(false);
                
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