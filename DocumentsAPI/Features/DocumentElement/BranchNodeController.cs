using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Documents.DocumentElement.Interface;
using Application.Documents.DocumentElement.Models;
using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.DocumentElement
{
    [Authorize, ApiController, Route("documentsapi/[controller]")]
    public class BranchNodeController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IBranchNodeService branchNodeService;
        private readonly ITokenService tokenService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public BranchNodeController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IBranchNodeService>().TryGetTarget(out branchNodeService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(GetType());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewNode([FromQuery]string elementID, [FromQuery]string branchID,
                                                       [FromQuery]string nodeName, [FromQuery]string comment)
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId branchObjectID = ObjectId.Parse(branchID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                var elementDto = await branchNodeService.CreateNewNodeAsync(elementObjectID, branchObjectID, userID,
                    nodeName, comment).ConfigureAwait(false);

                return new OkJson<DocumentElementDto>(elementDto);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("This id is not in correct format");
            }
            catch (ArgumentException ex)
            {
                return new BadSentRequest<string>(ex.Message);
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
        public async Task<IActionResult> UpdateNodeProperties([FromQuery]string elementID, [FromQuery]string branchNodeID) 
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId branchNodeObjectID = ObjectId.Parse(branchNodeID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                IDictionary<string, string> requestBody = await requestService.GetRequestBody<Dictionary<string, string>>(Request)
                    .ConfigureAwait(false);

                var elementDto = await branchNodeService.UpdateNodePropertiesAsync(elementObjectID, branchNodeObjectID, userID,
                    requestBody["name"], requestBody["comment"]).ConfigureAwait(false);

                return new OkJson<DocumentElementDto>(elementDto);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("This id is not in correct format");
            }
            catch (ArgumentException ex)
            {
                return new BadSentRequest<string>(ex.Message);
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

        [HttpPut("content")]
        public async Task<IActionResult> UpdateNodeContent([FromQuery]string elementID, [FromQuery]string branchNodeID) 
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId branchNodeObjectID = ObjectId.Parse(branchNodeID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                string requestBody = await requestService.GetRequestBody(Request).ConfigureAwait(false);

                var elementDto = await branchNodeService.UpdateContentAsync(new DocumentElementContentUpdateDto()
                {
                    BranchNodeID = branchNodeObjectID,
                    DocumentElementID = elementObjectID,
                    NewContent = requestBody,
                    UserID = userID,
                });

                return new OkJson<DocumentElementDto>(elementDto);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("This id is not in correct format");
            }
            catch (ArgumentException ex)
            {
                return new BadSentRequest<string>(ex.Message);
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
        public async Task<IActionResult> DeleteNode([FromQuery]string elementID, [FromQuery]string branchNodeID) 
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId branchNodeObjectID = ObjectId.Parse(branchNodeID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                var elementDto = await branchNodeService.DeleteNodeAsync(elementObjectID, branchNodeObjectID, userID)
                    .ConfigureAwait(false);

                return new OkJson<DocumentElementDto>(elementDto);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("This id is not in correct format");
            }
            catch (ArgumentException ex)
            {
                return new BadSentRequest<string>(ex.Message);
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