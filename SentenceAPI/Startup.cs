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
using SentenceAPI.FactoriesManager;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Databases.MongoDB.Factories;
using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Features.Loggers.Factories;
using SentenceAPI.Features.Loggers.Interfaces;
using SentenceAPI.Middlewares.RequestLoggerMiddleware;
using SentenceAPI.Features.Email.Services;
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

            app.UseMiddleware<RequestLogger>();

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
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
            factoriesManager.AddFactory(new FactoryInfo(new MongoDBServiceFactory(),
                typeof(IMongoDBServiceFactory)));
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
        }
    }
}
