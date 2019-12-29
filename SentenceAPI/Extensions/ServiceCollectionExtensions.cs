using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SentenceAPI.Features.Authentication.Models;

namespace SentenceAPI.Extensions
{
    /// <summary>
    /// These extension methods are used to set up service collection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void SetAuthentication(this IServiceCollection services)
        {
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
                        LifetimeValidator = AuthOptions.GetLifeTimeValidationDel(),

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });
        }

        public static void SetMvc(this IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false).
                SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}