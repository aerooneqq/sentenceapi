using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Codes.Factories;
using Application.Email.Factories;
using Application.Links.Factories;
using Application.Requests.Factories;
using Application.Responses.Factories;
using Application.Tokens.Factories;
using Application.UserActivity.Factories;
using Application.UserFeed.Factories;
using Application.UserFriends.Factories;
using Application.UserPhoto.Factories;
using Application.Users.Factories;
using Application.Workplace.DocumentsDeskState.Factories;
using Application.Workplace.DocumentStorage.FolderService.Factories;
using Application.Workplace.DocumentStorage.Services.Factories;
using Application.Workplace.DocumentStorage.UserMainFoldersService.Factories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Models;
using SharedLibrary.FactoriesManager.Interfaces;

using SentenceAPI.ApplicationFeatures.Loggers.Factories;
using SentenceAPI.ApplicationFeatures.DefferedExecution;
using SentenceAPI.Extensions;
using SentenceAPI.ApplicationFeatures.Middlewares;
using SentenceAPI.StartupHelperClasses;

using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;

using Domain.Date;

using Newtonsoft.Json;
using Application.Hash.Factories;
using Application.Caching.Factories;
using Application.Documents.Documents.Factories;

namespace SentenceAPI
{
    public class Startup
    {
        public static string ApiName => "SentenceAPI";
        public static string CurrDirectory => Directory.GetCurrentDirectory();
        public static Dictionary<OtherApis, string> OtherApis { get; private set; }


        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            OtherApis = new Dictionary<OtherApis, string>();
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

            ReadOtherApisConfigFile("./configs/other_api.json").GetAwaiter().GetResult();
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
            factoriesManager.AddFactory(new FactoryInfo(new ResponseServiceFactory(),
                typeof(ResponseServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentDeskStateServiceFactory(),
                typeof(DocumentDeskStateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserPhotoServiceFactory(),
                typeof(UserPhotoServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FileServiceFactory(), 
                typeof(FileServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FolderServiceFactory(),
                typeof(FolderServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserMainFoldersServiceFactory(), 
                typeof(UserMainFoldersServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(DateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new HashServiceFactory(), typeof(HashServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new CacheServiceFactory(), typeof(CacheServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentServiceFactory(), typeof(DocumentServiceFactory)));
        }

        private async Task ReadOtherApisConfigFile(string filePath)
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new StreamReader(fs);

            OtherApis = JsonConvert.DeserializeObject<Dictionary<OtherApis, string>>(await sr.ReadToEndAsync());
        }
    }
}
