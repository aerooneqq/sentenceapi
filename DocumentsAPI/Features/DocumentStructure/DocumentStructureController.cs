using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;
using DocumentsAPI.Features.DocumentStructure.Interfaces;
using DocumentsAPI.Features.DocumentStructure.Models;
using DocumentsAPI.Models.DocumentStructureModels;
using DocumentsAPI.Features.DocumentStructure.Validators;
using DocumentsAPI.Features.DocumentStructure.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;


namespace DocumentsAPI.Features.DocumentStructure 
{
    [ApiController, Route("api/[controller]"), Authorize]
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
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentStructure([FromQuery]ObjectId documentID)
        { 
            try 
            { 
                var documentStructure = await documentStructureService.GetDocumentStructureAsync(documentID)
                    .ConfigureAwait(false);

                if (documentStructure is null)
                {
                    return new BadSendedRequest<string>("There is no structure for this document");
                }

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
                {
                    return new BadSendedRequest<string>("Update info was sent in a bad format");
                }

                var documentStructure = await documentStructureService.GetDocumentStructureAsync(
                    itemUpdateDto.ParentDocumentStructureID).ConfigureAwait(false);

                if (documentStructure is null)
                {
                    return new BadSendedRequest<string>("Document structure with given ID does not exist");
                }

                var validationResult = new ItemUpdateDtoValidator(itemUpdateDto, documentStructure).Validate();
                if (!validationResult.result)
                {
                    #warning Add logging for validation errors
                    return new BadSendedRequest<string>(validationResult.errorMessage);
                }

                await documentStructureService.UpdateStructureAsync(documentStructure, itemUpdateDto).
                    ConfigureAwait(false);

                return new Ok();
            }
            catch (ItemNotFoundException ex)
            {
                return new BadSendedRequest<string>(ex.Message);
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