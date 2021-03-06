﻿using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;
using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using Application.Workplace.DocumentStorage.FileService.Interfaces;
using Application.Workplace.DocumentStorage.FileService.Models;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Validators;
using Domain.Workplace.DocumentsStorage;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Interfaces;

using MongoDB.Bson;
using System.Net;
using SentenceAPI.StartupHelperClasses;
using Domain.Models.Document;
using System.IO;

namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [Route("sentenceapi/[controller]"), Authorize, ApiController]
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
                    return new BadSentRequest<string>(errorMessage);
                }

                var createdFileID = await fileService.CreateNewFileAsync(userID, newFile.ParentFolderObjectId, newFile.FileName)
                    .ConfigureAwait(false);
                var file = await fileService.GetFileAsync(createdFileID).ConfigureAwait(false);

                file.DocumentID = await SendCreationRequestToDocumentsAPI(createdFileID, userID, 
                    file.FileName, DocumentType.UserOnly).ConfigureAwait(false);

                await fileService.UpdateAsync(file);

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

        private async Task<ObjectId> SendCreationRequestToDocumentsAPI(ObjectId fileID, ObjectId userID, 
                                                                       string fileName, DocumentType documentType) 
        {
            try
            {
                var documentsApiUrl = $"{Startup.OtherApis[OtherApis.DocumentsAPI]}/documentsapi/fileToDocument?fileID={fileID}&" +
                              $"userID={userID}&fileName={fileName}&documentType={documentType}";

                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(documentsApiUrl);
                request.Method = "PUT";

                HttpWebResponse response = (HttpWebResponse) (await request.GetResponseAsync().ConfigureAwait(false));

                string content = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                return ObjectId.Parse(content.Substring(1, content.Length - 2));
            }
            catch (WebException) 
            {
                throw new DatabaseException("Error occured while creating the document for file");
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
                    return new BadSentRequest<string>(errorMsg);
                }

                await fileService.RenameFileAsync(fileRenameDto.FolderID, fileRenameDto.FileName)
                    .ConfigureAwait(false);

                return new Ok();
            }
            catch (ArgumentException)
            {
                return new BadSentRequest<string>("Such file does not exist");
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
