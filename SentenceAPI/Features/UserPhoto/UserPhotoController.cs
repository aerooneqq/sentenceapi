using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Threading.Tasks;
using System.Text;

using MongoDB.Bson;
using SharedLibrary.Loggers.Configuration;


namespace SentenceAPI.Features.UserPhoto
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class UserPhotoController : ControllerBase
    {
        #region Services
        private ILogger<ApplicationError> exceptionLogger;
        private IUserPhotoService userPhotoService;
        private IRequestService requestService;
        private ITokenService tokenService;
        #endregion


        public UserPhotoController(IMemoryCache memoryCache, IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            exceptionLogger.LogConfiguration = new LogConfiguration(this.GetType());

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

                byte[] photo = await userPhotoService.GetRawPhotoAsync(userPhoto.CurrentPhotoID).ConfigureAwait(false);

                return new OkJson<byte[]>(photo, Encoding.UTF8);
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
        public async Task<IActionResult> UpdatePhotoAsync()
        {
            try
            {
                byte[] photo = await requestService.GetRequestBody<byte[]>(Request).ConfigureAwait(false);

                if (photo is null || photo.Length == 0)
                { 
                    return new BadSendedRequest<string>("There is no photo");
                }

                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                Models.UserPhoto userPhoto = await userPhotoService.GetPhotoAsync(userID).ConfigureAwait(false);

                if (userPhoto is null)
                {
                    userPhoto = new Models.UserPhoto(userID);
                    await userPhotoService.InsertUserPhotoModel(userPhoto).ConfigureAwait(false);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                return new InternalServerError();
            }
        }
    }
}
