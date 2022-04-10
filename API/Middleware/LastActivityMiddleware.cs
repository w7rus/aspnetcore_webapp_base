using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware;

public class LastActivityMiddleware
{
    private readonly RequestDelegate _next;

    public LastActivityMiddleware(
        RequestDelegate next
    )
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context,
        ILogger<LastActivityMiddleware> logger,
        IAppDbContextAction appDbContextAction,
        IUserService userService,
        IUserAdvancedService userAdvancedService
    )
    {
        logger.Log(LogLevel.Information, Localize.Log.MiddlewareForwardStart(GetType()));
        logger.Log(LogLevel.Information, Localize.Log.MiddlewareForwardEnd(GetType()));

        await _next(context);

        logger.Log(LogLevel.Information, Localize.Log.MiddlewareBackwardStart(GetType()));

        try
        {
            await appDbContextAction.BeginTransactionAsync();

            var user = await userAdvancedService.GetFromHttpContext();
            if (user == null)
                return;

            user.LastActivity = DateTimeOffset.UtcNow;
            user.LastIpAddress = context.Connection.RemoteIpAddress?.ToString();

            await userService.Save(user);

            await appDbContextAction.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await appDbContextAction.RollbackTransactionAsync();

            throw;
        }

        logger.Log(LogLevel.Information, Localize.Log.MiddlewareBackwardEnd(GetType()));
    }
}