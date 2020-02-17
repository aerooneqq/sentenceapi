using System;
using System.Threading.Tasks;

using Application.Documents.DocumentStructure.Exceptions;
using Application.Documents.DocumentStructure.Interfaces;
using Application.Documents.DocumentStructure.Models;

using DataAccessLayer.Exceptions;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;
using DocumentsAPI.Features.DocumentStructure.Validators;

using Domain.DocumentStructureModels;
using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.DocumentStructure 
{
    [ApiController, Route("documentsapi/[controller]"), Authorize]
    public class DocumentStructureController : Controller 
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentStructureService documentStructureService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;

        
        public DocumentStructureController(IFactoriesManager factoriesManager)
        { 
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());

            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentStructure([FromQuery]string documentID)
        { 
            try 
            { 
                if (documentID is null) 
                    return new BadSentRequest<string>("Document id can not be null");

                ObjectId documentObjectId = ObjectId.Parse(documentID);

                var documentStructure = await documentStructureService.GetDocumentStructureAsync(documentObjectId)
                    .ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("There is no structure for this document");

                return new OkJson<DocumentStructureModel>(documentStructure);
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
        public async Task<IActionResult> PutItemInDocumentStructure()
        {
            try 
            {
                var itemUpdateDto = await requestService.GetRequestBodyAsync<ItemUpdateDto>(Request)
                    .ConfigureAwait(false);

                if (itemUpdateDto is null)
                    return new BadSentRequest<string>("Update info was sent in a bad format");

                var documentStructure = await documentStructureService.GetDocumentStructureAsync(
                    itemUpdateDto.ParentDocumentStructureID).ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("Document structure with given ID does not exist");

                var validationResult = new ItemUpdateDtoValidator(itemUpdateDto, documentStructure).Validate();
                if (!validationResult.result)
                {
                    #warning Add logging for validation errors
                    return new BadSentRequest<string>(validationResult.errorMessage);
                }

                await documentStructureService.UpdateStructureAsync(documentStructure, itemUpdateDto).
                    ConfigureAwait(false);

                return new Ok();
            }
            catch (ItemNotFoundException ex)
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