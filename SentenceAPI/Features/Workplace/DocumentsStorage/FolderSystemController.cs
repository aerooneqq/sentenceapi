﻿using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

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
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Date.Interfaces;
using MongoDB.Bson;

namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("api/[controller]"), ApiController, Authorize]
    public class FolderSystemController : ControllerBase
    {
        #region Services
        private IRequestService requestService;
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IFileService fileService;
        private IFolderService folderService;
        private IDateService dateService;
        #endregion
        

        public FolderSystemController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
        }

        [HttpGet]
        public async Task<IActionResult> GetFoldersAndDocuments([FromQuery]ObjectId folderID)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                IEnumerable<DocumentFile> documentFiles = (await fileService.GetFilesAsync(userID, folderID)
                    .ConfigureAwait(false));

                IEnumerable<DocumentFolder> documentFolders = (await folderService.GetFolders(userID, folderID)
                    .ConfigureAwait(false));

                return new OkJson<FolderSystemDto>(new FolderSystemDto(documentFiles, documentFolders));
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

        /// <summary>
        /// Searches for the files and folders which satisfy the given query.
        /// </summary>
        [HttpGet, Route("search")]
        public async Task<IActionResult> GetFodlersAndFiles([FromQuery]string query)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                IEnumerable<DocumentFile> files = await fileService.GetFilesAsync(userID, query)
                    .ConfigureAwait(false);

                IEnumerable<DocumentFolder> folders = await folderService.GetFolders(userID, query)
                    .ConfigureAwait(false);

                return new OkJson<FolderSystemDto>(new FolderSystemDto(files, folders));
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

        /// <summary>
        /// Places the second folder in the first folder.
        /// </summary>
        [HttpPut, Route("folders")]
        public async Task<IActionResult> PutFolderIntoAnotherFolder([FromQuery]ObjectId firstFolderID,
                                                                    [FromQuery]ObjectId secondFolderID)
        {
            try
            {
                if (firstFolderID == secondFolderID)
                {
                    return new BadSendedRequest<string>("Folders can not be the same");
                }

                var firstFolder = await folderService.GetFolderData(firstFolderID).ConfigureAwait(false);
                var secondFolder = await folderService.GetFolderData(secondFolderID).ConfigureAwait(false);

                if (firstFolder is null || secondFolder is null)
                {
                    return new BadSendedRequest<string>("Folders with such ids do not exist");
                }

                secondFolder.ParentFolderID = firstFolder.ID;

                secondFolder.LastUpdateDate = dateService.GetCurrentDate();
                firstFolder.LastUpdateDate = dateService.GetCurrentDate();

                await folderService.Update(secondFolder).ConfigureAwait(false);

                return new Ok();
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

        [HttpPut, Route("files")]
        public async Task<IActionResult> PutFileInFolder([FromQuery]ObjectId fileID, [FromQuery]ObjectId folderID)
        {
            try
            {
                var file = await fileService.GetFileAsync(fileID).ConfigureAwait(false);
                var folder = await folderService.GetFolderData(folderID).ConfigureAwait(false);

                if (file is null || folder is null)
                {
                    return new BadSendedRequest<string>("The file or folder with such and id does not exist");
                }

                file.ParentFolderID = folder.ID;
                file.LastUpdateDate = folder.LastUpdateDate = dateService.GetCurrentDate();

                await fileService.UpdateAsync(file).ConfigureAwait(false);
                await folderService.Update(folder).ConfigureAwait(false);

                return new Ok();
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
