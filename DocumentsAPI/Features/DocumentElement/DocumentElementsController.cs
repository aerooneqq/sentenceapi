using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Documents.DocumentElement.Interface;
using Application.Documents.DocumentElement.Models;
using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using DataAccessLayer.Exceptions;

using Domain.DocumentElements.Dto;
using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.DocumentElement
{
    [Authorize, ApiController, Route("documentsapi/[controller]")]
    public class DocumentElementsController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentElementService documentElementService;
        private readonly ITokenService tokenService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public DocumentElementsController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentElementService>().TryGetTarget(out documentElementService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetItemContent([FromQuery]string itemID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                ObjectId itemObjectID = ObjectId.Parse(itemID);

                var itemElements = await documentElementService.GetDocumentElementsAsync(itemObjectID, userID).ConfigureAwait(false);

                return new OkJson<IEnumerable<DocumentElementDto>>(itemElements);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("The id is not in correct format");
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

        [HttpPost]
        public async Task<IActionResult> CreateDocumentElement([FromQuery]string documentID, [FromQuery]string itemID, 
                                                               [FromQuery]int type) 
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                ObjectId documentObjectID = ObjectId.Parse(documentID);
                ObjectId itemObjectID = ObjectId.Parse(itemID);

                DocumentElementCreateDto dto = new DocumentElementCreateDto(documentObjectID, userID, itemObjectID, type);
                await documentElementService.CreateNewDocumentElementAsync(dto).ConfigureAwait(false);

                var itemElements = await documentElementService.GetDocumentElementsAsync(itemObjectID, userID).ConfigureAwait(false);
                
                return new OkJson<IEnumerable<DocumentElementDto>>(itemElements);
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
        public async Task<IActionResult> UpdateDocumentElement([FromQuery]string documentElementID)
        {
            try
            {
                ObjectId documentElementObjectID = ObjectId.Parse(documentElementID);

                return new Ok();
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("This id is not in correct format");
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