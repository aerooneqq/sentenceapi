using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;
using DocumentsAPI.Features.Documents.Events;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Models.Document;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.Events;
using SharedLibrary.Events.Exceptions;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using Application.Documents.Documents.Interfaces;
using Application.Tokens.Interfaces;


namespace DocumentsAPI.Features.Documents
{
    [Route("documentsapi/[controller]"), Authorize, ApiController]
    public class DocumentsController : Controller
    {
        private readonly IFactoriesManager factoriesManager;
        
        #region Services
        private readonly IDocumentService documentService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public DocumentsController(IFactoriesManager factoriesManager)
        {
            this.factoriesManager = factoriesManager;
            
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(GetType());
        }


        [HttpPost]
        public async Task<IActionResult> CreateEmptyDocument([FromQuery] string documentName, 
            [FromQuery] DocumentType documentType)
        {
            try
            {
                if (documentName is null) 
                {
                    return new BadSentRequest<string>("Document name must be set");
                }

                ObjectId userID = requestService.GetUserID(Request);

                ObjectId documentID = await documentService.CreateEmptyDocument(userID, documentName, documentType).
                    ConfigureAwait(false);

                await EventManager.Raise(new DocumentCreationEvent(documentID, documentName, userID, factoriesManager)).
                    ConfigureAwait(false);

                return new Created(documentID.ToString());
            }
            catch (DomainEventException ex)
            {
                return new InternalServerError(ex.Message);
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

        [HttpGet]
        public async Task<IActionResult> GetDocumentInfo([FromQuery] string documentID)
        {
            try
            {
                ObjectId documentObjectID = ObjectId.Parse(documentID);

                return new OkJson<Document>(await documentService.GetDocumentByID(documentObjectID)
                    .ConfigureAwait(false));
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

        [HttpDelete]
        public async Task<IActionResult> DeleteDocumentByID([FromQuery] string documentID)
        {
            try
            {
                ObjectId documentObjectID = ObjectId.Parse(documentID);

                await documentService.DeleteDocumentByID(documentObjectID).ConfigureAwait(false);

                return new Ok();
            }
            catch (ArgumentException ex)
            {
                return new BadSentRequest<string>(ex.Message);
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