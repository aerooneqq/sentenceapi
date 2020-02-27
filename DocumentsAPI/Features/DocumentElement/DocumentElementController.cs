using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Documents.DocumentElement.Interface;

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
    public class DocumentElementController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentElementService documentElementService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public DocumentElementController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentElementService>().TryGetTarget(out documentElementService);

            logConfiguration = new LogConfiguration(GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetItemContent([FromQuery]string itemID)
        {
            try
            {   
                ObjectId itemObjectID = ObjectId.Parse(itemID);

                var itemElements = await documentElementService.GetDocumentElements(itemObjectID).ConfigureAwait(false);

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

        [HttpPut]
        public async Task<IActionResult> UpdateDocumentElement([FromQuery]string documentElementID)
        {
            try
            {
                ObjectId documentElementObjectID = ObjectId.Parse(documentElementID);

                
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