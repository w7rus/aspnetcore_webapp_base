using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
using BLL.Services.Advanced;
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
    private readonly IUserToUserGroupAdvancedService _userToUserGroupAdvancedService;
    private readonly IUserAdvancedService _userAdvancedService;

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
        IUserGroupService userGroupService,
        IUserToUserGroupAdvancedService userToUserGroupAdvancedService,
        IUserAdvancedService userAdvancedService
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
        _userToUserGroupAdvancedService = userToUserGroupAdvancedService;
        _userAdvancedService = userAdvancedService;
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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new CustomException();
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Received {user.GetType().Name} {user.Id}"));

            var fileInfo = new FileInfo(formFile.FileName);
            var fileName = Guid.NewGuid() + fileInfo.Extension;
            var ms = new MemoryStream();
            await formFile.OpenReadStream().CopyToAsync(ms, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Copied {ms.Length} bytes from form file"));

            //Get file creation Permission that will be compared
            var permission = await _permissionService.GetByAliasAsync("uint64_file_create_power");
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Received {permission.GetType().Name} {permission.Alias}"));
            
            //Get file creation comparable system PermissionValue
            var permissionValueComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias("uint64_file_create_power_needed_system",
                    cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Received System {permissionValueComparedSystem.GetType().Name} {permissionValueComparedSystem.Id}"));
            
            //Authorize file creation
            if (!await _userToUserGroupAdvancedService.AuthorizePermission(user, permission, permissionValueComparedSystem,
                    cancellationToken))
                throw new CustomException(Localize.Error.PermissionInsufficientPermissions);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"{permissionValueComparedSystem.Permission.Alias} authorized {permission.Alias}"));

            //Get AgeRating model field mapping Permission that will be compared
            var permissionFileCreateAutomapFileAgerating =
                await _permissionService.GetByAliasAsync("uint64_filecreate_automap_file.agerating_power");
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Received Permission {permissionFileCreateAutomapFileAgerating.Alias}"));

            //Get AgeRating model field mapping comparable system PermissionValue
            var permissionValueFileCreateAutomapFileAgeratingComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "uint64_filecreate_automap_file.agerating_power_needed_system");
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Received System PermissionValue {permissionValueFileCreateAutomapFileAgeratingComparedSystem.Id}"));

            //Map with conditional authorization. Mapping configuration profile is located at BLL.Maps.AutoMapperProfile
            var file = _mapper.Map<File>(data, opts =>
            {
                opts.Items["user"] = user;
                opts.Items["modelFieldMappingPermissionAuthorizationTuples"] =
                    new Dictionary<string, AutoMapperComparePermissionToTuple>
                    {
                        {
                            nameof(File.AgeRating), new AutoMapperComparePermissionToTuple
                            {
                                ComparedPermission = permissionFileCreateAutomapFileAgerating,
                                ComparablePermissionValue = null,
                                ComparablePermissionValueSystem = permissionValueFileCreateAutomapFileAgeratingComparedSystem,
                                ComparableCustomValue = null
                            }
                        }
                    };
            });
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"{data.GetType().Name} mapped to {file.GetType().Name}"));

            file.Name = fileName;
            file.Data = ms.ToArray();
            file.Metadata = new Dictionary<string, string>();
            file.UserId = user.Id;
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Set additional data to {file.GetType().Name}"));

            file = await _fileService.Create(file, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Create), $"Staged creation of {file.GetType().Name} {file.Id}"));

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Create)));

            return new FileCreateResult
            {
                Id = file.Id
            };
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
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
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Read), $"Received {file.GetType().Name} {file.Id}"));

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
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
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
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Read), $"Received {file.GetType().Name} {file.Id}"));

            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Read), $"Staging deletion of {file.GetType().Name} {file.Id}"));
            await _fileService.Delete(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Delete)));

            return new FileDeleteResult();
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}