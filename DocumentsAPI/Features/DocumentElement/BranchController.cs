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
    public class BranchController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IBranchService branchService;
        private readonly ITokenService tokenService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;
        

        public BranchController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IBranchService>().TryGetTarget(out branchService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(GetType());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBranch([FromQuery]string elementID, [FromQuery]string branchName) 
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                var elementDto = await branchService.CreateNewBranchAsync(elementObjectID, branchName, userID)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteBranch([FromQuery]string elementID, [FromQuery]string branchID) 
        {
            try
            {
                ObjectId elementObjectID = ObjectId.Parse(elementID);
                ObjectId branchObjectID = ObjectId.Parse(branchID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                var elementDto = await branchService.DeleteBranchAsync(elementObjectID, branchObjectID, userID)
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