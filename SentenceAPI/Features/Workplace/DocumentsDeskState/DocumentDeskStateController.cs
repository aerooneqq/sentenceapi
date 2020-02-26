using System;
using System.Threading.Tasks;
using System.Linq;

using Application.Requests.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Workplace.DocumentsDeskState;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Extensions;
using SharedLibrary.Loggers.Interfaces;

using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;

using MongoDB.Bson;
using Application.Documents.Documents.Interfaces;
using Domain.Models.Document;
using System.Collections.Generic;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState
{
    [ApiController, Route("sentenceapi/[controller]"), Authorize]
    public class DocumentDeskStateController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentDeskStateService deskStateService;
        private readonly IRequestService requestService;
        private readonly IDocumentService documentService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public DocumentDeskStateController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentDeskStateService>().TryGetTarget(out deskStateService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);

            logConfiguration = new LogConfiguration(this.GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentDeskState()
        {
            try
            {
                var deskState = await deskStateService.GetDeskStateAsync(requestService.GetToken(Request))
                    .ConfigureAwait(false);

                return new OkJson<DocumentDeskState>(deskState);
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
        public async Task<IActionResult> PutFileToTopBar([FromQuery] string documentID)
        {
            try
            {
                ObjectId documentObjectID = ObjectId.Parse(documentID);
                var token = requestService.GetToken(Request);
                var deskState = await deskStateService.GetDeskStateAsync(token).ConfigureAwait(false);

                if (deskState.DocumentTopBarInfos is null)
                    deskState.DocumentTopBarInfos = new List<DocumentTopBarInfo>();

                if (deskState.DocumentTopBarInfos.Contains(d => d.DocumentID == documentObjectID))
                {
                    return new NoContent();
                }

                Document document = await documentService.GetDocumentByID(documentObjectID).ConfigureAwait(false);

                deskState.DocumentTopBarInfos = deskState.DocumentTopBarInfos.Append(new DocumentTopBarInfo()
                {
                    DocumentID = documentObjectID,
                    DocumentName = document.Name
                });

                await deskStateService.UpdateDeskStateAsync(deskState).ConfigureAwait(false);

                return Ok();
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
        public async Task<IActionResult> DeleteFileFromTopBar(ObjectId documentID)
        {
            try
            {
                string token = requestService.GetToken(Request);
                var deskState = await deskStateService.GetDeskStateAsync(token).ConfigureAwait(false);

                if (!deskState.DocumentTopBarInfos.Contains(d => d.DocumentID == documentID))
                {
                    return new NoContent();
                }

                deskState.DocumentTopBarInfos.ToList().RemoveAll(d => d.DocumentID == documentID);

                await deskStateService.UpdateDeskStateAsync(deskState).ConfigureAwait(false);

                return Ok();
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
