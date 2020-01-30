using System;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Workplace.DocumentsStorage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;

using MongoDB.Bson;


namespace SentenceAPI.Features.Workplace.DocumentsStorage
{
    [ApiController, Authorize, Route("api/[controller]")]
    public class UserMainFoldersController : Controller
    {
        #region Services
        private readonly IUserMainFoldersService userMainFoldersService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public UserMainFoldersController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserMainFoldersService>().TryGetTarget(out userMainFoldersService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMainFolders()
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var userMainFolders = await userMainFoldersService.GetUserMainFoldersAsync(userID)
                    .ConfigureAwait(false);

                return new OkJson<UserMainFolders>(userMainFolders);
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