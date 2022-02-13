using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BrunoZell.ModelBinding;
using Common.Models;
using DTO.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
public class FileController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<FileController> _logger;
    private readonly IFileHandler _fileHandler;

    #endregion

    #region Ctor

    public FileController(ILogger<FileController> logger, IFileHandler fileHandler)
    {
        _logger = logger;
        _fileHandler = fileHandler;
    }

    #endregion

    #region Methods

    [HttpPost]
    [SwaggerOperation(Summary = "Creates file",
        Description = "Creates file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [Required] [FromForm] [ModelBinder(BinderType = typeof(JsonModelBinder))]
        FileCreate data,
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Create(data, file, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads file",
        Description = "Reads file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] FileRead data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Read(data, cancellationToken));
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes file",
        Description = "Deletes file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] FileDelete data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Delete(data, cancellationToken));
    }

    #endregion
}