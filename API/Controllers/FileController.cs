using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services;
using BLL.Services.Advanced;
using BrunoZell.ModelBinding;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using DTO.Models.File;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;
using AuthenticationSchemes = Common.Models.AuthenticationSchemes;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<FileController> _logger;
    private readonly IFileHandler _fileHandler;

    #endregion

    #region Ctor

    public FileController(
        ILogger<FileController> logger,
        IFileHandler fileHandler,
        IHttpContextAccessor httpContextAccessor,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _fileHandler = fileHandler;
    }

    #endregion

    #region Methods

    [DisableFormValueModelBinding]
    [RequestSizeLimit(134217728L)] //128MB
    [RequestFormLimits(MultipartBodyLengthLimit = 134217728L)] //128MB
    [HttpPost]
    [SwaggerOperation(Summary = "Creates file",
        Description = "Creates file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(FileCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartExpected);
        }

        var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        var multipartSection = await reader.ReadNextSectionAsync(cancellationToken);

        if (multipartSection == null)
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionNotFound);

        if (!ContentDispositionHeaderValue.TryParse(
                multipartSection.ContentDisposition, out var contentDispositionForm))
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Request,
                Localize.Error.RequestMultipartSectionContentDispositionParseFailed);

        if (!MultipartRequestHelper.HasFormDataContentDisposition(contentDispositionForm))
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionContentDispositionFormExpected);

        var encoding = multipartSection.GetEncoding();

        if (encoding == null)
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionEncodingNotSupported);

        using var streamReader = new StreamReader(multipartSection.Body, encoding,
            detectEncodingFromByteOrderMarks: true, bufferSize: 1024);

        var data = await JsonSerializer.DeserializeAsync<FileCreateDto>(streamReader.BaseStream,
                       cancellationToken: cancellationToken) ??
                   throw new CustomException(Localize.Error.ObjectDeserializationFailed);

        multipartSection = await reader.ReadNextSectionAsync(cancellationToken);

        if (multipartSection == null)
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionNotFound);

        if (!ContentDispositionHeaderValue.TryParse(
                multipartSection.ContentDisposition, out var contentDispositionFile))
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Request,
                Localize.Error.RequestMultipartSectionContentDispositionParseFailed);

        if (!MultipartRequestHelper.HasFileContentDisposition(contentDispositionFile))
            throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Request,
                Localize.Error.RequestMultipartSectionContentDispositionFormExpected);

        await using var fileStream = multipartSection.Body;

        return ResponseWith(await _fileHandler.Create(data,
            WebUtility.HtmlEncode(contentDispositionFile.FileName.Value), fileStream, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads file",
        Description = "Reads file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] FileReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Read(data, cancellationToken));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Updates file",
        Description = "Updates file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(FileUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] FileUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes file",
        Description = "Deletes file")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] FileDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _fileHandler.Delete(data, cancellationToken));
    }

    #endregion
}