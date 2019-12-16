using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DataAccessLayer.Exceptions;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Models;

namespace SentenceAPI.Features.Workplace.DocumentsDeskState
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class DocumentDeskStateController : ControllerBase
    {
        #region Factories
        private IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IDocumentDeskStateService deskStateService;
        private IRequestService requestService;
        #endregion

        public DocumentDeskStateController()
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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }
    }
}
