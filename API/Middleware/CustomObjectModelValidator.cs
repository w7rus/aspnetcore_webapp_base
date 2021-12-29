#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace API.Middleware;

public class CustomObjectModelValidator : IObjectModelValidator
{
    public void Validate(
        ActionContext actionContext,
        ValidationStateDictionary? validationState,
        string prefix,
        object? model
    )
    {
    }
}