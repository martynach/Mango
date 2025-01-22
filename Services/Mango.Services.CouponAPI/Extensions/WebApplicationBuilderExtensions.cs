using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.CouponAPI.Extensions;

public static class WebApplicationBuilderExtensions
{

    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = builder.Configuration.GetSection("Authentication:JwtIssuer").Value,
                ValidAudience = builder.Configuration.GetSection("Authentication:JwtAudience").Value,
                // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:JwtKey").Value)) // it would also work
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Authentication:JwtKey").Value))
            };
        });

        return builder;
    }
}
