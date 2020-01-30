using DataAccessLayer.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using System;
using System.Threading.Tasks;
using System.Text;

using Domain.Logs;
using Domain.Logs.Configuration;
using MongoDB.Bson;


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

        private readonly LogConfiguration logConfiguration;


        public UserPhotoController(IMemoryCache memoryCache, IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            logConfiguration = new LogConfiguration(this.GetType());
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
                    return new BadSentRequest<string>("Upload your photo firstly!");
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
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
                    return new BadSentRequest<string>("There is no photo");
                }

                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                Domain.UserPhoto.UserPhoto userPhoto = await userPhotoService.GetPhotoAsync(userID).ConfigureAwait(false);

                if (userPhoto is null)
                {
                    userPhoto = new Domain.UserPhoto.UserPhoto(userID);
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
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}
