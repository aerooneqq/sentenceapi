using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using Application.Workplace.DocumentStorage.FileService.Interfaces;
using Application.Workplace.DocumentStorage.FolderService.Interfaces;
using Application.Workplace.DocumentStorage.FolderService.Models;

using Domain.Date;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Workplace.DocumentsStorage;

using MongoDB.Bson;


namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("sentenceapi/[controller]"), ApiController, Authorize]
    public class FolderSystemController : ControllerBase
    {
        #region Services
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFileService fileService;
        private readonly IFolderService folderService;
        private readonly IDateService dateService;
        #endregion
        
        private readonly LogConfiguration logConfiguration;

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
        public async Task<IActionResult> GetFoldersAndDocuments([FromQuery]string folderID)
        {
            try
            {
                ObjectId id = ObjectId.Parse(folderID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                IEnumerable<DocumentFile> documentFiles = (await fileService.GetFilesAsync(userID, id)
                    .ConfigureAwait(false));

                IEnumerable<DocumentFolder> documentFolders = (await folderService.GetFolders(userID, id)
                    .ConfigureAwait(false));

                return new OkJson<FolderSystemDto>(new FolderSystemDto(documentFiles, documentFolders));
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        /// <summary>
        /// Places the second folder in the first folder.
        /// </summary>
        [HttpPut, Route("folders")]
        public async Task<IActionResult> PutFolderIntoAnotherFolder([FromQuery]string firstFolderID,
                                                                    [FromQuery]string secondFolderID)
        {
            try
            {
                ObjectId firstFolderObjectID = ObjectId.Parse(firstFolderID);
                ObjectId secondFolderObjectID = ObjectId.Parse(secondFolderID);

                if (firstFolderObjectID == secondFolderObjectID)
                {
                    return new BadSentRequest<string>("Folders can not be the same");
                }

                var firstFolder = await folderService.GetFolderData(firstFolderObjectID).ConfigureAwait(false);
                var secondFolder = await folderService.GetFolderData(secondFolderObjectID).ConfigureAwait(false);

                if (firstFolder is null || secondFolder is null)
                {
                    return new BadSentRequest<string>("Folders with such ids do not exist");
                }

                secondFolder.ParentFolderID = firstFolder.ID;

                secondFolder.LastUpdateDate = dateService.Now;
                firstFolder.LastUpdateDate = dateService.Now;

                await folderService.Update(secondFolder).ConfigureAwait(false);

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

        [HttpPut, Route("files")]
        public async Task<IActionResult> PutFileInFolder([FromQuery]string fileID, 
                                                         [FromQuery]string folderID)
        {
            try
            {
                ObjectId fileObjectID = ObjectId.Parse(fileID);
                ObjectId folderObjectID = ObjectId.Parse(folderID);

                var file = await fileService.GetFileAsync(fileObjectID).ConfigureAwait(false);
                var folder = await folderService.GetFolderData(folderObjectID).ConfigureAwait(false);

                if (file is null || folder is null)
                {
                    return new BadSentRequest<string>("The file or folder with such and id does not exist");
                }

                file.ParentFolderID = folder.ID;
                file.LastUpdateDate = folder.LastUpdateDate = dateService.Now;

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}
