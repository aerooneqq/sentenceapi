using DocumentsAPI.Features.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DocumentsAPI.Extensions.ServiceCollectionExtensions
{
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
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = true,
                        LifetimeValidator = AuthOptions.GetLifeTimeValidationDel(),

                        IssuerSigningKey = AuthOptions.GetSecurityKey(),
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