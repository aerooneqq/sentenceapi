using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.ApplicationFeatures.Date.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
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
    public class FolderSystemController : ControllerBase
    {
        #region Static fields
        private static LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = "DocumentFolderSystemController",
            ServiceName = string.Empty
        };
        #endregion

        #region Services
        private IRequestService requestService;
        private ITokenService tokenService;
        private ILogger<ApplicationError> exceptionLogger;
        private IFileService fileService;
        private IFolderService folderService;
        private IDateService dateService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public FolderSystemController()
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> GetFoldersAndDocuments([FromQuery]long folderID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
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
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }

        /// <summary>
        /// Places the second folder in the first folder.
        /// </summary>
        [HttpPut, Route("replaceFolder")]
        public async Task<IActionResult> PutFolderIntoAnotherFolder([FromQuery]long firstFolderID,
                                                                    [FromQuery]long secondFolderID)
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

                await folderService.Update(secondFolder);

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

        [HttpPut, Route("replaceFile")]
        public async Task<IActionResult> PutFileInFolder([FromQuery]long fileID, [FromQuery]long folderID)
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
                await exceptionLogger.Log(new ApplicationError(ex)).ConfigureAwait(false);
                return new InternalServerError();
            }
        }
    }
}
