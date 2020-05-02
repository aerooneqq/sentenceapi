using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

using Application.Tokens.Interfaces;
using Application.UserActivity.Interfaces;
using Application.Users.Interfaces;

using DataAccessLayer.Exceptions;
using DataAccessLayer.Hashes;
using Domain.Authentication;
using Domain.Date;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;

using Microsoft.AspNetCore.Mvc;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace AuthorizationServer.Features.Authorization
{
    [ApiController, Route("authorization")]
    public class AuthorizationController
    {
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IUserService<UserInfo> userService;
        private readonly ITokenService tokenService;
        private readonly IUserActivityService userActivityService;
        private readonly IDateService dateService;

        private readonly LogConfiguration logConfiguration;


        public AuthorizationController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<IUserActivityService>().TryGetTarget(out userActivityService);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            
            logConfiguration = new LogConfiguration(GetType());
        }


        [HttpGet]
        public async Task<IActionResult> GetToken([FromQuery]string email, [FromQuery]string password)
        {
            try
            {
                UserInfo user = await userService.GetAsync(email, password.GetMD5Hash()).ConfigureAwait(false);

                if (user is null)
                {
                    return new BadSentRequest<string>("User with such email and pass does not exist");
                }

                (string encodedToken, JwtSecurityToken jwtSecurityToken) = tokenService.CreateEncodedToken(user);
                
                await tokenService.InsertTokenInDBAsync(new JwtToken(jwtSecurityToken, user)).ConfigureAwait(false);
                await userActivityService.AddSingleActivityAsync(user.ID, new Domain.UserActivity.SingleUserActivity() 
                {
                    ActivityDate = dateService.Now,
                    Activity = "Logged in"
                }).ConfigureAwait(false);

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