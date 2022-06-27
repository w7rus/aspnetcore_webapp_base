using System.Diagnostics;
using System.Linq;
using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Attributes;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var modelState = context.ModelState;

        if (modelState.IsValid) return;

        var errorModelResult = new ErrorModelResult
        {
            TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        };

        foreach (var modelError in context.ModelState.Values.SelectMany(modelStateValue => modelStateValue.Errors))
            errorModelResult.Errors.Add(new ErrorModelResultEntry(ErrorType.ModelState, modelError.ErrorMessage));

        context.Result = new BadRequestObjectResult(errorModelResult);
    }
}