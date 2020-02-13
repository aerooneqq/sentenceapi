using System.Collections.Generic;
using System.IO;
using Application.Requests.Factories;
using Application.Responses.Factories;
using Application.Tokens.Factories;
using Application.Users.Factories;
using AuthorizationServer.ApplicationFeatures.Loggers;
using AuthorizationServer.Middlewares;
using AuthorizationServer.Models;
using DataAccessLayer.DatabasesManager;
using DataAccessLayer.DatabasesManager.Interfaces;
using Domain.Authentication;
using Domain.Date;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager.Models;


namespace AuthorizationServer
{
    public class Startup
    {
        public static string CurrDirectory => Directory.GetCurrentDirectory();

        public static readonly IDictionary<Server, string> Servers = new Dictionary<Server, string>()
        {
            [Server.SentenceAPI] = "http://localhost:5000/sentenceapi",
            [Server.DocumentsAPI] = "http://localhost:6000/documentsapi"
        };


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IFactoriesManager factoriesManager = new FactoriesManager();
            IDatabaseManager databaseManager = new DatabasesManager();

            ConfigureCustomServices(factoriesManager, databaseManager);

            services.AddSingleton(typeof(IFactoriesManager), factoriesManager);
            services.AddSingleton(typeof(IDatabaseManager), databaseManager);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        LifetimeValidator = AuthOptions.GetLifeTimeValidationDel(),

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RedirectionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
        }

        private void ConfigureCustomServices(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.Inject(typeof(IDatabaseManager), databaseManager);
            factoriesManager.Inject(typeof(IFactoriesManager), factoriesManager);

            factoriesManager.AddFactory(new FactoryInfo(new TokenServiceFactory(), typeof(TokenServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new DateServiceFactory(), typeof(DateServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new UserServiceFactory(), typeof(UserServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new LoggerFactory(), typeof(LoggerFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new ResponseServiceFactory(), typeof(ResponseServiceFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new RequestServiceFactory(), typeof(RequestServiceFactory)));
        }
    }
}