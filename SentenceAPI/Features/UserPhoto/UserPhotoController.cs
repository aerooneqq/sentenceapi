using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using MongoDB.Bson;

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
        private readonly IFactoriesManager factoriesManager =
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        public UserPhotoController(IMemoryCache memoryCache)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = logConfiguration;

            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);

            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotoAsync()
        {
            try
            {
                var userPhoto = await userPhotoService.GetPhotoAsync(requestService.GetToken(Request)).
                    ConfigureAwait(false);

                if (userPhoto is null)
                {
                    return new BadSendedRequest<string>("Upload your photo firstly!");
                }

                byte[] photo = await userPhotoService.GetRawPhotoAsync(userPhoto.PhotoGridFSId);

                return new OkJson<byte[]>(photo, Encoding.UTF8);
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
        public async Task<IActionResult> UpdatePhotoAsync()
        {
            try
            {
                byte[] photo = await requestService.GetRequestBody<byte[]>(Request).ConfigureAwait(false);
                long userID = long.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                Models.UserPhoto userPhoto = await userPhotoService.GetPhotoAsync(userID);

                if (userPhoto is null)
                {
                    userPhoto = new Models.UserPhoto(userID, ObjectId.Empty);
                    await userPhotoService.InsertUserPhotoModel(userPhoto);
                }
                
                ObjectId newPhotoID = await userPhotoService.UpdatePhotoAsync(userPhoto, photo,
                    userPhotoService.GetUserPhotoName(userID)).ConfigureAwait(false);
                
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
