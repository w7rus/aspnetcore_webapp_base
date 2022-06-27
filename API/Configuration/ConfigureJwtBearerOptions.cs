using System.Text;
using API.AuthHandlers;
using Common.Models;
using Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Configuration;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JsonWebTokenAuthenticationSchemeOptions>
{
    private readonly JsonWebTokenOptions _jsonWebTokenOptions;

    public ConfigureJwtBearerOptions(IOptions<JsonWebTokenOptions> jsonWebTokenOptions)
    {
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
    }

    public void Configure(string name, JsonWebTokenAuthenticationSchemeOptions options)
    {
        options.TokenValidationParameters = name switch
        {
            AuthenticationSchemes.JsonWebToken => new TokenValidationParameters
            {
                ValidateIssuer = _jsonWebTokenOptions.ValidateIssuer,
                ValidateAudience = _jsonWebTokenOptions.ValidateAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = _jsonWebTokenOptions.ValidateIssuerSigningKey,
                ValidIssuer = _jsonWebTokenOptions.Issuer,
                ValidAudience = _jsonWebTokenOptions.Audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jsonWebTokenOptions.IssuerSigningKey))
            },
            AuthenticationSchemes.JsonWebTokenExpired => new TokenValidationParameters
            {
                ValidateIssuer = _jsonWebTokenOptions.ValidateIssuer,
                ValidateAudience = _jsonWebTokenOptions.ValidateAudience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = _jsonWebTokenOptions.ValidateIssuerSigningKey,
                ValidIssuer = _jsonWebTokenOptions.Issuer,
                ValidAudience = _jsonWebTokenOptions.Audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jsonWebTokenOptions.IssuerSigningKey))
            },
            _ => options.TokenValidationParameters
        };
    }

    public void Configure(JsonWebTokenAuthenticationSchemeOptions options)
    {
        Configure(string.Empty, options);
    }
}