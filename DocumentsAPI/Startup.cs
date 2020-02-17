using System.IO;

using Application.Documents.Documents.Factories;
using Application.Documents.DocumentStructure.Factories;
using Application.Documents.FileToDocument.Factories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager.Models;

using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;

using DocumentsAPI.ApplicationFeatures.Loggers.Factories;
using DocumentsAPI.ApplicationFeatures.Requests.Factories;
using DocumentsAPI.Extensions.AppExtensions;
using DocumentsAPI.Extensions.ServiceCollectionExtensions;

using Domain.Date;

using Application.Tokens.Factories;

namespace DocumentsAPI
{
    public class Startup
    {
        public static string CurrDirectory => Directory.GetCurrentDirectory();
        public static string ApiName => "DocumentsAPI";
        
        public void ConfigureServices(IServiceCollection services)
        {
            IFactoriesManager factoriesManager = new FactoriesManager();
            IDatabaseManager databaseManager = new DatabasesManager();
            
            ConfigureCustomServices(factoriesManager, databaseManager);

            services.AddSingleton(typeof(IFactoriesManager), factoriesManager);
            services.AddSingleton(typeof(IDatabaseManager), databaseManager);

            services.AddMvc(options => 
            {
                options.EnableEndpointRouting = false;
            });
            
            services.SetAuthentication();
            services.SetMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpsRedirection();    
            app.UseAuthentication();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            
            app.SetForwardedHeaders();
            app.UseMvc();
        }

        private static void ConfigureCustomServices(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.Inject(typeof(IFactoriesManager), factoriesManager);
            factoriesManager.Inject(typeof(IDatabaseManager), databaseManager);

            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(DateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new RequestServiceFactory(), typeof(RequestServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FileToDocumentServiceFactory(), typeof(FileToDocumentServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentStructureServiceFactory(), typeof(DocumentStructureServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentServiceFactory(), typeof(DocumentServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(), typeof(LoggerFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new TokenServiceFactory(), typeof(TokenServiceFactory)));
        }
    }
}
