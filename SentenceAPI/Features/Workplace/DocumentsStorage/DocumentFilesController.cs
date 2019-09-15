using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.ActionResults;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using SentenceAPI.Validators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class DocumentFilesController : ControllerBase
    {
        #region Static fields
        private static LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = "DocumentFileController",
            ServiceName = string.Empty
        };
        #endregion

        #region Services
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFileService fileService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public DocumentFilesController()
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewFile()
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                NewFileDto newFile = await requestService.GetRequestBody<NewFileDto>(Request)
                    .ConfigureAwait(false);

                var (result, errorMessage) = new FileNameValidator(newFile.FileName).Validate();
                if (!result)
                {
                    return new BadSendedRequest<string>(errorMessage);
                }

                await fileService.CreateNewFile(userID, newFile.ParentFolderID, newFile.FileName)
                    .ConfigureAwait(false);

                return new Ok();
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

        [HttpDelete]
        public async Task<IActionResult> DeleteFile([FromQuery]long fileID)
        {
            try
            {
                await fileService.DeleteFile(fileID).ConfigureAwait(false);

                return new Ok();
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
