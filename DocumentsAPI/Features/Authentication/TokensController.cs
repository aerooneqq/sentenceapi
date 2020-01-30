using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;
using DocumentsAPI.ApplicationFeatures.Requests.Interfaces;
using DocumentsAPI.Features.Authentication.Interfaces;
using DocumentsAPI.Features.Authentication.Models;
using Domain.Logs;
using Domain.Logs.Configuration;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.Authentication
{
    [ApiController, Route("api/[controller]")]
    public class TokensController : Controller
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public TokensController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task<IActionResult> GetToken([FromQuery]string userID, [FromQuery]string sentenceAPITokenID,
                                                  [FromQuery]string requestID)
        {
            try
            {
                ObjectId userObjectID = ObjectId.Parse(userID);
                ObjectId sentenceAPITokenObjectID   = ObjectId.Parse(sentenceAPITokenID);
                ObjectId requestObjectID = ObjectId.Parse(requestID);
                
                if (!(await requestService.CheckIfRequestInDatabase(requestObjectID).
                    ConfigureAwait(false)))
                {
                    return new Unauthorized();
                }

                var (encodedToken, token) = tokenService.CreateEncodedToken(userObjectID, 
                    sentenceAPITokenObjectID, requestObjectID);
                var documentsJwtToken = new DocumentsJwtToken(userObjectID, sentenceAPITokenObjectID, token);
                
                await tokenService.InsertDocumentToken(documentsJwtToken).ConfigureAwait(false);
                
                return new OkJson<string>(encodedToken);
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