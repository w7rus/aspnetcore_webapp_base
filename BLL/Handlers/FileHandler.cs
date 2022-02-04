using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BLL.Handlers.Base;
using BLL.Services;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(FileCreate data, IFormFile formFile, CancellationToken cancellationToken = new());
    Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = new());
    Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = new());
}

public class FileHandler : HandlerBase, IFileHandler
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IFileService _fileService;
    private readonly HttpContext _httpContext;
    private readonly IUserService _userService;

    #endregion

    #region Ctor

    public FileHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IFileService fileService,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _fileService = fileService;
        _userService = userService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(
        FileCreate data,
        IFormFile formFile,
        CancellationToken cancellationToken = new()
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userService.GetFromHttpContext();
            if (user == null)
                throw new CustomException();

            var fileInfo = new FileInfo(formFile.FileName);
            var fileName = Guid.NewGuid() + fileInfo.Extension;
            var ms = new MemoryStream();
            await formFile.OpenReadStream().CopyToAsync(ms, cancellationToken);
            var file = await _fileService.Add(fileName, ms.ToArray(), data.AgeRating, new Dictionary<string, string>(),
                user.Id,
                cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Create)));

            return new FileCreateResult
            {
                Id = file.Id
            };
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Create), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.FileCreateFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = new())
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var file = await _fileService.GetByIdAsync(data.Id, cancellationToken);

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = file.Name,
                Inline = true,
            };
            _httpContext.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Read)));

            return new FileReadResult
            {
                Data = file.Data,
                ContentType = file.ContentType
            };
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Read), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.FileReadFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = new())
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Delete)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var file = await _fileService.GetByIdAsync(data.Id, cancellationToken);

            await _fileService.Delete(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Delete)));

            return new FileDeleteResult();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Delete), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.File, Localize.Error.FileDeleteFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.File, e.Message));

            return errorModelResult;
        }
    }

    #endregion
}