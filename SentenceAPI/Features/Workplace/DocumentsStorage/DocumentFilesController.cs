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
using MongoDB.Bson;
using SentenceAPI.Events;
using SentenceAPI.Features.Workplace.DocumentsStorage.Events;

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

        private readonly LogConfiguration logConfiguration;

        public DocumentFilesController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<IFileService>().TryGetTarget(out fileService);

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            logConfiguration = new LogConfiguration(this.GetType());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewFile()
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                NewFileDto newFile = await requestService.GetRequestBody<NewFileDto>(Request)
                    .ConfigureAwait(false);

                var (result, errorMessage) = new FileNameValidator(newFile.FileName).Validate();
                if (!result)
                {
                    return new BadSendedRequest<string>(errorMessage);
                }

                var createdFileID = await fileService.CreateNewFileAsync(userID, newFile.ParentFolderID, newFile.FileName)
                    .ConfigureAwait(false);
                var file = await fileService.GetFileAsync(createdFileID).ConfigureAwait(false);


                await EventManager.Raise(new FileCreationEvent(file, userID)).ConfigureAwait(false);

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

        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery]string fileID)
        {
            try
            {
                ObjectId fileObjectID = ObjectId.Parse(fileID);
                DocumentFile file = await fileService.GetFileAsync(fileObjectID).ConfigureAwait(false);

                return new OkJson<DocumentFile>(file);
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
        public async Task<IActionResult> DeleteFile([FromQuery]string fileID)
        {
            try
            {
                ObjectId fileObjectID = ObjectId.Parse(fileID);
                await fileService.DeleteFileAsync(fileObjectID).ConfigureAwait(false);

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}
