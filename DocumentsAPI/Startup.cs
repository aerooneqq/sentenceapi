using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;

using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.DatabasesManager;

namespace DocumentsAPI
{
    public class Startup
    {
        public static string ApiName => "DocumentsAPI";
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IFactoriesManager), ManagersDictionary.Instance.GetManager(ApiName));
            services.AddSingleton(typeof(IDatabaseManager), DatabasesManager.Manager);
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

    }
}
