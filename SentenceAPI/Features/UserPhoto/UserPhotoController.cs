using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SentenceAPI.ActionResults;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class UserPhotoController : ControllerBase
    {
        #region Static fields
        private static readonly LogConfiguration logConfiguration = new LogConfiguration()
        {
            ControllerName = "UserPhotoController",
            ServiceName = string.Empty
        };
        #endregion

        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserPhotoService userPhotoService;
        private IRequestService requestService;
        private ITokenService tokenService;
        #endregion

        #region Factories
        private readonly FactoriesManager.FactoriesManager factoriesManager =
            FactoriesManager.FactoriesManager.Instance;
        #endregion

        public UserPhotoController(IMemoryCache memoryCache)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;

            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);
            userPhotoService.MemoryCache = memoryCache;

            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhoto()
        {
            try
            {
                var userPhoto = await userPhotoService.GetPhoto(requestService.GetToken(Request)).ConfigureAwait(false);

                return new OkJson<Models.UserPhoto>(userPhoto);
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

        [HttpPut]
        public async Task<IActionResult> UpdatePhoto()
        {
            try
            {
                var photo = await requestService.GetRequestBody<byte[]>(Request).ConfigureAwait(false);
                var userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                await userPhotoService.UpdatePhoto(new Models.UserPhoto(userID, photo)).ConfigureAwait(false);

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
