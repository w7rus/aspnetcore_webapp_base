using System;
using System.Diagnostics;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Filters;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var errorModelResult = new ErrorModelResult
        {
            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        };

        switch (context.Exception)
        {
            case HttpResponseException httpResponseException:
                errorModelResult.Errors.Add(new ErrorModelResultEntry(httpResponseException.Type,
                    httpResponseException.Message));

                context.Result = new ObjectResult(errorModelResult)
                {
                    StatusCode = httpResponseException.StatusCode
                };

                context.ExceptionHandled = true;
                break;
            case CustomException customException:
                errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Generic,
                    customException.Message));

                context.Result = new ObjectResult(errorModelResult)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                context.ExceptionHandled = true;
                break;
            case { } exception:
                errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.Unhandled,
                    exception.Message));

                context.Result = new ObjectResult(errorModelResult)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                context.ExceptionHandled = true;
                break;
        }
    }

    public int Order => int.MaxValue - 10;
}