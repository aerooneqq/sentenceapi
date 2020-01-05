using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DataAccessLayer.Exceptions;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Extensions;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;
using SharedLibrary.Loggers.Configuration;
using MongoDB.Bson;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class DocumentDeskStateController : ControllerBase
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IDocumentDeskStateService deskStateService;
        private IRequestService requestService;
        #endregion

        public DocumentDeskStateController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentDeskStateService>().TryGetTarget(out deskStateService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutFileToTopBar(ObjectId documentID, string documentName)
        {
            try
            {
                var token = requestService.GetToken(Request);
                var deskState = await deskStateService.GetDeskStateAsync(token).ConfigureAwait(false);

                if (deskState.DocumentTopBarInfos.Contains(d => d.DocumentID == documentID))
                {
                    return new NoContent();
                }

                deskState.DocumentTopBarInfos.Append(new DocumentTopBarInfo()
                {
                    DocumentID = documentID,
                    DocumentName = documentName
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}
