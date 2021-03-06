using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Application.Requests.Interfaces;
using Application.Templates;
using Application.Templates.Interfaces;
using Application.Tokens.Interfaces;

using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Templates;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using Newtonsoft.Json;
using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.Templates
{
    [ApiController, Route("documentsapi/[controller]"), Authorize]
    public class TemplatesController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITemplateService templateService;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion


        private readonly LogConfiguration logConfiguration;


        public TemplatesController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITemplateService>().TryGetTarget(out templateService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(GetType());
        } 


        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedTemplates()
        {
            try
            {
                var templates = await templateService.GetPublishedTemplates().ConfigureAwait(false);
                return new OkJson<IEnumerable<TemplateDto>>(templates.OrderBy(template => template.DocumentCount));
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

        [HttpGet("user")]
        public async Task<IActionResult> GetUserTemplates()
        {
            try
            {
                ObjectId userObjectID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var templates = await templateService.GetUserTemplates(userObjectID).ConfigureAwait(false);
                return new OkJson<IEnumerable<TemplateDto>>(templates.OrderBy(template => template.DocumentCount));
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpGet("user/search")]
        public async Task<IActionResult> SearchForUserTemplates([FromQuery]string query) 
        {
            try
            {
                query = query ?? string.Empty;
                ObjectId userObjectID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var templates = await templateService.SearchForUserTemplates(userObjectID, query).ConfigureAwait(false);
                return new OkJson<IEnumerable<TemplateDto>>(templates.OrderBy(template => template.DocumentCount));
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpGet("published/search")]
        public async Task<IActionResult> SearchForPublishedTemplates([FromQuery]string query) 
        {
            try
            {
                query = query ?? string.Empty;
                var templates = await templateService.SearchForPublishedTemplates(query).ConfigureAwait(false);
                return new OkJson<IEnumerable<TemplateDto>>(templates.OrderBy(template => template.DocumentCount));
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpGet]
        public async Task<IActionResult> GetTemplate([FromQuery]string templateID)
        {
            try
            {
                ObjectId templateObjectID = ObjectId.Parse(templateID);
                var template = await templateService.GetTemplateByID(templateObjectID).ConfigureAwait(false);
                return new OkJson<TemplateDto>(template);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpPost]
        public async Task<IActionResult> CreateNewTemplate([FromBody]TemplateCreationDto dto)
        {
            try
            {
                dto.AuthorID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var createdTemplate = await templateService.CreateNewTemplate(dto).ConfigureAwait(false);
                return new OkJson<TemplateDto>(createdTemplate);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpPut]
        public async Task<IActionResult> UpdateTemplate()
        {
            try
            {
                var dto = JsonConvert.DeserializeObject<TemplateUpdateDto>(await requestService.GetRequestBody(Request));
                var createdTemplate = await templateService.UpdateTemplate(dto).ConfigureAwait(false);
                return new OkJson<TemplateDto>(createdTemplate);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpDelete]
        public async Task<IActionResult> DeleteTemplate([FromQuery]string templateID) 
        {
            try
            {
                ObjectId templateObjectID = ObjectId.Parse(templateID);
                await templateService.DeleteTemplate(templateObjectID).ConfigureAwait(false);

                return new Ok();
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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

        [HttpPut("documentsCount")]
        public async Task<IActionResult> IncreaseCreatedDocumentCount([FromQuery]string templateID)
        {
            try
            {
                ObjectId templateObjectID = ObjectId.Parse(templateID);
                var updatedTemplate = await templateService.IncreaseDocumentCountForTemplate(templateObjectID)
                    .ConfigureAwait(false);

                return new OkJson<TemplateDto>(updatedTemplate);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("userID is incorrect format");
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