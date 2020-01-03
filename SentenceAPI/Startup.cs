using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using System.IO;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Models;
using SharedLibrary.FactoriesManager.Interfaces;

using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.ApplicationFeatures.Loggers.Factories;
using SentenceAPI.Features.Email.Factories;
using SentenceAPI.Features.Links.Factories;
using SentenceAPI.Features.UserFriends.Factories;
using SentenceAPI.Features.UserActivity.Factories;
using SentenceAPI.Features.UserFeed.Factories;
using SentenceAPI.Features.Codes.Factories;
using SentenceAPI.ApplicationFeatures.Requests.Factories;
using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Factories;
using SentenceAPI.Features.UserPhoto.Factories;
using SentenceAPI.Features.Workplace.DocumentsStorage.Factories;
using SentenceAPI.ApplicationFeatures.Date.Factories;
using SentenceAPI.Extensions;
using SentenceAPI.ApplicationFeatures.Middlewares;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;

namespace SentenceAPI
{
    public class Startup
    {
        public static string ApiName => "SentenceAPI";
        public static string CurrDirectory => Directory.GetCurrentDirectory();

        #region Factories
        private readonly IFactoriesManager factoriesManager;
        #endregion

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ManagersDictionary.Instance.AddManager(ApiName);
            factoriesManager = ManagersDictionary.Instance.GetManager(ApiName);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCustomServices();

            services.SetAuthentication();
            services.SetMvc();
            services.AddSingleton(typeof(IFactoriesManager), ManagersDictionary.Instance.GetManager(ApiName));
            services.AddSingleton(typeof(IDatabaseManager), DatabasesManager.Manager);

            DefferedTasksManager.Initialize();
            DefferedTasksManager.Start();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ResponseLoggerMiddleware>();
            app.UseMiddleware<RequestLoggerMiddleware>();

            app.UseAuthentication();
            app.SetForwardedHeaders();
            app.SetCors();
            app.UseMvc();
        }

        /// <summary>
        /// This method initializes a factory manager, where all factories which will be needed in the system
        /// are stored. With a factories manager we can get access to any service in any part of the system.
        /// </summary>
        private void ConfigureCustomServices()
        {
            factoriesManager.Inject(typeof(IFactoriesManager), factoriesManager);
            factoriesManager.Inject(typeof(IDatabaseManager), DatabasesManager.Manager);
            
            factoriesManager.AddFactory(new FactoryInfo(new UserServiceFactory(),
                typeof(UserServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new TokenServiceFactory(),
                typeof(TokenServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(),
                typeof(LoggerFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new EmailServiceFactory(),
                typeof(EmailServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LinkServiceFactory(),
                typeof(LinkServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserFriendsServiceFactory(),
                typeof(UserFriendsServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserActivityServiceFactory(),
                typeof(UserActivityServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserFeedServiceFactory(),
                typeof(UserFeedServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new CodesServiceFactory(),
                typeof(CodesServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new RequestServiceFactory(),
                typeof(RequestServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentDeskStateServiceFactory(),
                typeof(DocumentDeskStateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserPhotoServiceFactory(),
                typeof(UserPhotoServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FolderSystemServiceFactory(),
                typeof(FolderSystemServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(DateServiceFactory)));
        }
    }
}
