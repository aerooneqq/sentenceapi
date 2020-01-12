using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager.Models;

using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;

using DocumentsAPI.ApplicationFeatures.Date.Factories;
using DocumentsAPI.ApplicationFeatures.Requests.Factories;
using DocumentsAPI.Features.FileToDocument.Factories;
using DocumentsAPI.Features.DocumentStructure.Factories;
using DocumentsAPI.Features.Documents.Factories;


namespace DocumentsAPI
{
    public class Startup
    {
        public static string ApiName => "DocumentsAPI";
        
        public void ConfigureServices(IServiceCollection services)
        {
            IFactoriesManager factoriesManager = new FactoriesManager();
            IDatabaseManager databaseManager = new DatabasesManager();

            services.AddSingleton(typeof(IFactoriesManager), factoriesManager);
            services.AddSingleton(typeof(IDatabaseManager), databaseManager);

            services.AddMvc();

            ConfigureCustomServices(factoriesManager, databaseManager);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

        public void ConfigureCustomServices(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.Inject(typeof(IFactoriesManager), factoriesManager);
            factoriesManager.Inject(typeof(IDatabaseManager), databaseManager);

            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(DateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new RequestServiceFactory(), typeof(RequestServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new FileToDocumentServiceFactory(), typeof(FileToDocumentServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentStructureServiceFactory(), typeof(DocumentStructureServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DocumentServiceFactory(), typeof(DocumentServiceFactory)));
        }
    }
}
