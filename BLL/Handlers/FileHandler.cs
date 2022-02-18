using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using Domain.Entities;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using File = Domain.Entities.File;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(FileCreate data, IFormFile formFile, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = default);
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
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly IUserGroupPermissionValueService _userGroupPermissionValueService;
    private readonly IUserGroupService _userGroupService;

    #endregion

    #region Ctor

    public FileHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IFileService fileService,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        IMapper mapper,
        IPermissionService permissionService,
        IUserGroupPermissionValueService userGroupPermissionValueService,
        IUserGroupService userGroupService
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _fileService = fileService;
        _userService = userService;
        _mapper = mapper;
        _permissionService = permissionService;
        _userGroupPermissionValueService = userGroupPermissionValueService;
        _userGroupService = userGroupService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(
        FileCreate data,
        IFormFile formFile,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new CustomException();

            var fileInfo = new FileInfo(formFile.FileName);
            var fileName = Guid.NewGuid() + fileInfo.Extension;
            var ms = new MemoryStream();
            await formFile.OpenReadStream().CopyToAsync(ms, cancellationToken);
            
            var permission = await _permissionService.GetByAliasAsync("uint64_file_create_power");
            var file = _mapper.Map<File>(data, opts =>
            {
                opts.Items["user"] = user;
                opts.Items["permission"] = permission;
                opts.Items["valueCompared"] = BitConverter.GetBytes(Consts.MemberUserGroupPowerBase);
            });

            file.Name = fileName;
            file.Data = ms.ToArray();
            file.Metadata = new Dictionary<string, string>();
            file.UserId = user.Id;
            
            file = await _fileService.Create(file, cancellationToken);

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

    public async Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = default)
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

    public async Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = default)
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