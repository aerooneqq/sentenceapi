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

using SharedLibrary.Events;

using DocumentsAPI.Features.FileToDocument.Events;


namespace DocumentsAPI.Features.FileToDocument
{
    [Route("documentsapi/[controller]"), ApiController]
    public class FileToDocumentController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentService documentService;
        #endregion

        private readonly IFactoriesManager factoriesManager;

        private readonly LogConfiguration logConfiguration;


        public FileToDocumentController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            
            this.factoriesManager = factoriesManager;

            logConfiguration = new LogConfiguration(GetType());
        }

        [HttpPut]
        public async Task<IActionResult> AssociateFileWithDocument(
            [FromQuery]string fileID, [FromQuery]string userID,
            [FromQuery]string fileName, [FromQuery]DocumentType documentType
            )
        {
            try 
            {
                if (fileID is null || userID is null || fileName is null)
                    return new BadSentRequest<string>("All parameters must be set");

                ObjectId fileObjectId = ObjectId.Parse(fileID);
                ObjectId userObjectId = ObjectId.Parse(userID);

                ObjectId documentID = await documentService.CreateEmptyDocument(userObjectId, fileName, documentType);

                await EventManager.Raise(new FileToDocumentAssociated(documentID, userObjectId, 
                                                                      fileName, factoriesManager));
                
                return new OkJson<string>(documentID.ToString());
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