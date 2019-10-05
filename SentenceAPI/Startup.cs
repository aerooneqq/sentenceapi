using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.FactoriesManager.Models;
using SentenceAPI.FactoriesManager.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.ApplicationFeatures.Loggers.Factories;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.Middlewares.RequestLoggerMiddleware;
using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Email.Factories;
using SentenceAPI.Features.Links.Factories;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.UserFriends.Factories;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.UserActivity.Factories;
using SentenceAPI.Features.UserActivity.Interfaces;
using SentenceAPI.Features.UserFeed.Factories;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.Codes.Factories;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.ApplicationFeatures.Requests.Factories;
using SentenceAPI.ApplicationFeatures.Requests.Interfaces;
using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Factories;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using System.Threading;
using SentenceAPI.Features.UserPhoto.Factories;
using SentenceAPI.Features.UserPhoto.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Factories;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SentenceAPI.ApplicationFeatures.Date.Factories;
using SentenceAPI.ApplicationFeatures.Date.Interfaces;

namespace SentenceAPI
{
    public class Startup
    {
        #region Services
        private ITokenService tokenService;
        #endregion

        #region Factories
        private readonly FactoriesManager.FactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;
        #endregion

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            DefferedTasksManager.Initialize();
            DefferedTasksManager.Start();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCustomServices();

            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        LifetimeValidator = tokenService.GetLifeTimeValidationDel(),

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddMemoryCache();

            services.AddMvc(options => options.EnableEndpointRouting = false).
                SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RequestLogger>();

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseMvc();
        }

        /// <summary>
        /// This method initializes a factory manager, where all factories which will be needed in the system
        /// are stored. With a factories manager we can get access to any service in any part of the system.
        /// </summary>
        private void ConfigureCustomServices()
        {
            IFactoriesManager factoriesManager = FactoriesManager.FactoriesManager.Instance;

            factoriesManager.AddFactory(new FactoryInfo(new UserServiceFactory(),
                typeof(IUserServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new TokenServiceFactory(),
                typeof(ITokenServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(),
                typeof(ILoggerFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new EmailServiceFactory(),
                typeof(IEmailServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LinkServiceFactory(),
                typeof(ILinkServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserFriendsServiceFactory(),
                typeof(IUserFriendsServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserActivityServiceFactory(),
                typeof(IUserActivityServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserFeedServiceFactory(),
                typeof(IUserFeedServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new CodesServiceFactory(),
                typeof(ICodesServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new RequestServiceFactory(),
                typeof(IRequestServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentDeskStateServiceFactory(),
                typeof(IDocumentDeskStateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserPhotoServiceFactory(),
                typeof(IUserPhotoServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FolderSystemServiceFactory(),
                typeof(IFolderSystemServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(IDateServiceFactory)));
        }
    }
}
