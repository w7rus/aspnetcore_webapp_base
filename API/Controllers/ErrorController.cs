using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace API.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    #region Fields

    private readonly IHostEnvironment _hostEnvironment;

    #endregion
    
    #region Ctor

    public ErrorController(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    #endregion

    #region Methods

    [Route("/Error")]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;
        
        var errorModelResult = new ErrorModelResult
        {
            Errors = new List<KeyValuePair<string, string>>
            {
                new(Localize.ErrorType.UnhandledException, Localize.Error.UnhandledExceptionContactSystemAdministrator)
            },
            TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        
        if (!_hostEnvironment.IsDevelopment())
        {
            return StatusCode(StatusCodes.Status500InternalServerError, errorModelResult);
        }
        
        errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.UnhandledExceptionErrorMessage, exceptionHandlerFeature.Error.Message));
        errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.UnhandledExceptionErrorStackTrace, exceptionHandlerFeature.Error.StackTrace));
        errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.UnhandledExceptionErrorSource, exceptionHandlerFeature.Error.Source));
        errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.UnhandledExceptionRequestPath, HttpUtility.UrlEncode(exceptionHandlerFeature.Path)));

        return StatusCode(StatusCodes.Status500InternalServerError, errorModelResult);
    }

    #endregion
}