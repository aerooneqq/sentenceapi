using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;
using SharedLibrary.Loggers.Configuration;
using SharedLibrary.Date.Interfaces;

using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using SentenceAPI.Validators;

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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}