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
        #endregion

        #region Factories
        private IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public FolderSystemController()
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);
            factoriesManager.GetService<IFolderService>().TryGetTarget(out folderService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> GetFoldersAndDocuments([FromQuery]long folderID)
        {
            try
            {
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                IEnumerable<DocumentFile> documentFiles = (await fileService.GetFiles(userID, folderID)
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

                IEnumerable<DocumentFile> files = await fileService.GetFiles(userID, query)
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
    }
}
