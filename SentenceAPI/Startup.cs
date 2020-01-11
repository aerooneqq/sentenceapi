using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

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

using Newtonsoft.Json;


namespace SentenceAPI
{
    public class Startup
    {
        public static string ApiName => "SentenceAPI";
        public static string CurrDirectory => Directory.GetCurrentDirectory();
        public static Dictionary<string, string> OtherApis { get; private set; }


        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            OtherApis = new Dictionary<string, string>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.SetAuthentication();
            services.SetMvc();

            IFactoriesManager factoriesManager = new FactoriesManager();
            IDatabaseManager databaseManager = new DatabasesManager();

            ConfigureCustomServices(factoriesManager, databaseManager);
            
            services.AddSingleton(typeof(IFactoriesManager), factoriesManager);
            services.AddSingleton(typeof(IDatabaseManager), databaseManager);

            DefferedTasksManager.Initialize(factoriesManager);
            DefferedTasksManager.Start();

            ReadOtherApisConfigFile("other_api.json").GetAwaiter().GetResult();
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
        private void ConfigureCustomServices(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.Inject(typeof(IFactoriesManager), factoriesManager);
            factoriesManager.Inject(typeof(IDatabaseManager), databaseManager);
            
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

        private async Task ReadOtherApisConfigFile(string filePath)
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new StreamReader(fs);

            OtherApis = JsonConvert.DeserializeObject<Dictionary<string, string>>(await sr.ReadToEndAsync());
        }
    }
}
