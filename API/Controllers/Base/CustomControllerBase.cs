using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Models;
using Common.Models.Base;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base;

[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status500InternalServerError)]
public class CustomControllerBase : ControllerBase
{
    #region Fields

    private readonly HttpContext _httpContext;
    private readonly IWarningAdvancedService _warningAdvancedService;

    #endregion

    #region Ctor

    public CustomControllerBase(IHttpContextAccessor httpContextAccessor, IWarningAdvancedService warningAdvancedService)
    {
        _warningAdvancedService = warningAdvancedService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    internal IActionResult ResponseWith(DTOResultBase response)
    {
        response.TraceId = Activity.Current?.Id ?? _httpContext.TraceIdentifier;

        response.Warnings = _warningAdvancedService.GetAll();

        if (response.Errors != null && response.Errors.Any())
            return new BadRequestObjectResult(response);
        if (response is FileReadResultDto result)
            return File(result.FileStream, result.ContentType, result.FileName);

        return new OkObjectResult(response);
    }

    #endregion
}