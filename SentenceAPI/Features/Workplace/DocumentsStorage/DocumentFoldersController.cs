using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using SentenceAPI.Validators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Date.Interfaces;
using MongoDB.Bson;

namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class DocumentFoldersController : ControllerBase
    {
        #region Services
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFolderService folderService;
        private readonly IDateService dateService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public DocumentFoldersController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            logConfiguration = new LogConfiguration(this.GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetFolderData([FromQuery]string folderID)
        {
            try
            {
                ObjectId folderObjectID = ObjectId.Parse(folderID);
                DocumentFolder documentFolder = await folderService.GetFolderData(folderObjectID).ConfigureAwait(false);

                return new OkJson<DocumentFolder>(documentFolder);
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
        public async Task<IActionResult> CreateNewFolder()
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(
                    requestService.GetToken(Request), "ID"));

                NewFolderDto newFolder = await requestService.GetRequestBody<NewFolderDto>(Request)
                    .ConfigureAwait(false);

                var (result, errorMessage) = new FolderNameValidator(newFolder.FolderName).Validate();
                if (!result)
                {
                    return new BadSendedRequest<string>(errorMessage);
                }

                await folderService.CreateFolder(userID, newFolder.ParentFolderID, newFolder.FolderName)
                    .ConfigureAwait(false);

                return new Ok();
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
        public async Task<IActionResult> DeleteFolder([FromQuery]string folderID)
        {
            try
            {
                ObjectId folderObjectId = ObjectId.Parse(folderID);
                await folderService.DeleteFolder(folderObjectId).ConfigureAwait(false);

                return new Ok();
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
        public async Task<IActionResult> RenameFolder([FromQuery]string folderID)
        {
            try
            {
                ObjectId folderObjectId = ObjectId.Parse(folderID);

                var newFolderName = await requestService.GetRequestBody<Dictionary<string, string>>(Request).
                    ConfigureAwait(false);

                await folderService.RenameFolder(folderObjectId, newFolderName["newFolderName"]).ConfigureAwait(false);

                return new Ok();
            }
            catch (ArgumentException)
            {
                return new BadSendedRequest<string>("The folder which such id does not exist");
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
