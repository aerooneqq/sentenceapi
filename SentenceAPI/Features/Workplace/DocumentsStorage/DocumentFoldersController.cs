﻿using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.ActionResults;
using SentenceAPI.ApplicationFeatures.Date.Interfaces;
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
    [Route("api/[controller]"), ApiController, Authorize]
    public class DocumentFoldersController : ControllerBase
    {
        #region Static fields
        private static LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = "DocumentFoldersController",
            ServiceName = string.Empty
        };
        #endregion

        #region Services
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFolderService folderService;
        private readonly IDateService dateService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public DocumentFoldersController()
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> GetFolderData([FromQuery]long folderID)
        {
            try
            {
                DocumentFolder documentFolder = await folderService.GetFolderData(folderID).ConfigureAwait(false);

                return new OkJson<DocumentFolder>(documentFolder);
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

        [HttpPost]
        public async Task<IActionResult> CreateNewFolder()
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(
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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteFolder([FromQuery]long folderID)
        {
            try
            {
                await folderService.DeleteFolder(folderID).ConfigureAwait(false);

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

        [HttpPut]
        public async Task<IActionResult> RenameFolder([FromQuery]long folderID)
        {
            try
            {
                var newFolderName = await requestService.GetRequestBody<Dictionary<string, string>>(Request).
                    ConfigureAwait(false);

                await folderService.RenameFolder(folderID, newFolderName["newFolderName"]).ConfigureAwait(false);

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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }
    }
}