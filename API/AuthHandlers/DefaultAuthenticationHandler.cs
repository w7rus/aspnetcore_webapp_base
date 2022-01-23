using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.AuthHandlers;

public class DefaultAuthenticationHandler : AuthenticationHandler<DefaultAuthenticationSchemeOptions>
{
    public DefaultAuthenticationHandler(
        IOptionsMonitor<DefaultAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.NoResult());
        
        var executingEndpoint = Context.GetEndpoint();

        return Task.FromResult(executingEndpoint == null
            ? AuthenticateResult.Fail(new NullReferenceException(nameof(executingEndpoint)))
            : AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name)));
    }
}