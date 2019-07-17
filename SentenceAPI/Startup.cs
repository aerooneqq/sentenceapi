using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using SentenceAPI.Features.Authentication.Models;
using SentenceAPI.Features.FactoriesManager.Models;
using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Users.Services;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Users.Factories;
using SentenceAPI.KernelInterfaces;
using SentenceAPI.Features.Authentication.Services;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Features.Response.Factories;
using SentenceAPI.Features.Response.Interfaces;

namespace SentenceAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method initializes a factory manager, where all factories which will be needed in the system
        /// are stored. With a factory we can get access to any service in any part of the system.
        /// </summary>
        public void ConfigureCustomServices()
        {
            IFactoryManager factoryManager = FactoriesManager.Instance;

            factoryManager.AddFactory(new FactoryInfo(new UserServiceFactory(), typeof(IUserService<UserInfo>)));
            factoryManager.AddFactory(new FactoryInfo(new TokenServiceFactory(), typeof(ITokenService)));
            factoryManager.AddFactory(new FactoryInfo(new ResponseServiceFactory(), typeof(IResponseService)));
        }

        public void ConfigureServices(IServiceCollection services)
        {
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
            app.UseMvc();

            ConfigureCustomServices();
        }
    }
}
