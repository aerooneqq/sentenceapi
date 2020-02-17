using System;
using System.Threading.Tasks;
using System.Net;
using SentenceAPI.StartupHelperClasses;
using System.IO;

using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;
using Application.UserActivity.Interfaces;
using Application.Users.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Features.Authentication.Dto;

using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;

using Domain.Authentication;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.UserActivity;
using Domain.Users;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.ActionResults;
using SharedLibrary.Loggers.Interfaces;

using MongoDB.Bson;


namespace SentenceAPI.Features.Authentication
{
    [Route("sentenceapi/[controller]")]
    [ApiController]
    public class TokensController : Controller
    {
        #region Services
        private readonly IUserService<UserInfo> userService;
        private readonly ITokenService tokenService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IUserActivityService userActivityService;
        private readonly IRequestService requestService;
        #endregion

        private readonly LogConfiguration logConfiguration; 
        

        #region Constructors
        public TokensController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);

            logConfiguration = new LogConfiguration(GetType());
        }
        #endregion


        #region Controller's method
        /// <summary>
        /// If the user with given email, password exists then this method returns
        /// the jwt token for this user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string email, [FromQuery]string password)
        {
            try
            {
                if (email is null || password is null) 
                { 
                    return new BadSentRequest<string>("Email and password must be defined");
                }
                
                password = password.GetMD5Hash();

                UserInfo user = await userService.GetAsync(email, password).ConfigureAwait(false);

                if (user == null || user.IsAccountDeleted)
                {
                    return Unauthorized();
                }

                ObjectId requestID = await LogRequestAndGetID(Request).ConfigureAwait(false);

                var (sentenceApiToken, securityToken) = tokenService.CreateEncodedToken(user);

                JwtToken jwtToken = new JwtToken(securityToken, user);
                await tokenService.InsertTokenInDBAsync(new JwtToken(securityToken, user)).ConfigureAwait(false);

                string documentsApiToken = await GetDocumentsApiToken(user, jwtToken.ID, requestID).
                    ConfigureAwait(false);

                AddLogInTaskToDefferedManager(user.ID);

                return new OkJson<AuthorizarionTokens>(new AuthorizarionTokens(sentenceApiToken, documentsApiToken));
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

        private async Task<string> GetDocumentsApiToken(UserInfo user, ObjectId sentenceAPITokenID,
                                                        ObjectId requestID)
        {
            string url = $"{Startup.OtherApis[OtherApis.DocumentsAPI]}/api/" + 
                $"tokens?userID={user.ID}&sentenceAPITokenID={sentenceAPITokenID}&requestID={requestID}";
            HttpWebRequest request = HttpWebRequest.CreateHttp(url);

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);

            using StreamReader streamReader = new StreamReader(response.GetResponseStream());
            
            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
        }

        private async Task<ObjectId> LogRequestAndGetID(HttpRequest request)
        {
            return await requestService.LogRequestToDatabase(request).ConfigureAwait(false);
        }

        private void AddLogInTaskToDefferedManager(ObjectId userID) =>
            DefferedTasksManager.AddTask(new Action(() => userActivityService.AddSingleActivityAsync(userID,
                new SingleUserActivity()
                {
                    ActivityDate = DateTime.Now,
                    Activity = "Logged in"
                })));
        #endregion
    }
}
