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
using System.Threading.Tasks;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;

namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("api/[controller]"), Authorize, ApiController]
    public class DocumentFilesController : ControllerBase
    {
        #region Services
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IFileService fileService;
        #endregion

        #region Factories
        private readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public DocumentFilesController()
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());
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

                await fileService.CreateNewFileAsync(userID, newFile.ParentFolderID, newFile.FileName)
                    .ConfigureAwait(false);

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

        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery]long fileID)
        {
            try
            {
                DocumentFile file = await fileService.GetFileAsync(fileID).ConfigureAwait(false);

                return new OkJson<DocumentFile>(file);
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
        public async Task<IActionResult> DeleteFile([FromQuery]long fileID)
        {
            try
            {
                await fileService.DeleteFileAsync(fileID).ConfigureAwait(false);

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

        [HttpPut]
        public async Task<IActionResult> RenameFile()
        {
            try
            {
                FileRenameDto fileRenameDto = await requestService.GetRequestBody<FileRenameDto>(Request)
                    .ConfigureAwait(false);

                var (result, errorMsg) = new FileNameValidator(fileRenameDto.FileName).Validate();
                if (!result)
                {
                    return new BadSendedRequest<string>(errorMsg);
                }

                await fileService.RenameFileAsync(fileRenameDto.FolderID, fileRenameDto.FileName)
                    .ConfigureAwait(false);

                return new Ok();
            }
            catch (ArgumentException)
            {
                return new BadSendedRequest<string>("Such file does not exist");
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
