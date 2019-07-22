using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.FactoriesManager.Models;
using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Features.Response.Factories;
using SentenceAPI.Features.Response.Interfaces;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Factories;
using SentenceAPI.Features.Loggers.Interfaces;

namespace SentenceAPI
{
    public class Startup
    {
        #region Services
        private ITokenService tokenService;
        #endregion

        #region Factories
        private readonly FactoriesManager factoriesManager = FactoriesManager.Instance;
        #endregion

        public IConfiguration Configuration { get; set;  }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureCustomServices();
            tokenService = (factoriesManager[typeof(ITokenServiceFactory)].Factory as ITokenServiceFactory)
                .GetService();

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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseMvc();
        }

        /// <summary>
        /// This method initializes a factory manager, where all factories which will be needed in the system
        /// are stored. With a factory we can get access to any service in any part of the system.
        /// </summary>
        private void ConfigureCustomServices()
        {
            IFactoriesManager factoryManager = FactoriesManager.Instance;

            factoryManager.AddFactory(new FactoryInfo(new UserServiceFactory(), 
                typeof(IUserServiceFactory)));
            factoryManager.AddFactory(new FactoryInfo(new TokenServiceFactory(), 
                typeof(ITokenServiceFactory)));
            factoryManager.AddFactory(new FactoryInfo(new ResponseServiceFactory(), 
                typeof(IResponseServiceFactory)));
            factoryManager.AddFactory(new FactoryInfo(new MongoDBServiceFactory(),
                typeof(IMongoDBServiceFactory)));
            factoryManager.AddFactory(new FactoryInfo(new LoggerFactory(),
                typeof(ILoggerFactory)));
        }
    }
}
