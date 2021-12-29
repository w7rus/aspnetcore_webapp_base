using System.Text;
using Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Configuration
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JsonWebTokenOptions _jsonWebTokenOptions;

        public ConfigureJwtBearerOptions(IOptions<JsonWebTokenOptions> jsonWebTokenOptions)
        {
            _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = _jsonWebTokenOptions.ValidateIssuer,
                    ValidateAudience = _jsonWebTokenOptions.ValidateAudience,
                    ValidateLifetime = _jsonWebTokenOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = _jsonWebTokenOptions.ValidateIssuerSigningKey,
                    ValidIssuer = _jsonWebTokenOptions.Issuer,
                    ValidAudience = _jsonWebTokenOptions.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jsonWebTokenOptions.IssuerSigningKey))
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }
    }
}