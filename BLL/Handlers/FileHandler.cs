using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
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
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var fileInfo = new FileInfo(formFile.FileName);
            var fileName = Guid.NewGuid() + fileInfo.Extension;
            var ms = new MemoryStream();
            await formFile.OpenReadStream().CopyToAsync(ms, cancellationToken);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create), $"Copied {ms.Length} bytes from form file"));

            //Get file creation Permission that will be compared
            var permission = await _permissionService.GetByAliasAsync("uint64_file_create_power");

            //Get file creation comparable system PermissionValue
            var permissionValueComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "uint64_file_create_power_needed_system",
                    cancellationToken);

            //Authorize file creation
            if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToAnyPermissionValue(user, permission,
                    permissionValueComparedSystem,
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            //Get AgeRating model field mapping Permission that will be compared
            var permissionFileCreateAutomapFileAgerating =
                await _permissionService.GetByAliasAsync("uint64_filecreate_automap_file.agerating_power");

            //Get AgeRating model field mapping comparable system PermissionValue
            var permissionValueFileCreateAutomapFileAgeratingComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "uint64_filecreate_automap_file.agerating_power_needed_system", cancellationToken);

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
                                ComparablePermissionValueSystem =
                                    permissionValueFileCreateAutomapFileAgeratingComparedSystem,
                                ComparableCustomValue = null
                            }
                        }
                    };
            });

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {file.GetType().Name}"));

            file.Name = fileName;
            file.Data = ms.ToArray();
            file.Size = file.Data.Length;
            file.Metadata = new Dictionary<string, string>();
            file.UserId = user.Id;

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create), $"Set additional data to {file.GetType().Name}"));

            file = await _fileService.Save(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Create)));

            return new FileCreateResult
            {
                Id = file.Id
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var file = await _fileService.GetByIdAsync(data.Id, cancellationToken);

            //Get file creation Permission that will be compared
            var permission = await _permissionService.GetByAliasAsync("uint64_file_read_power");

            //Get file creation comparable PermissionValue
            var permissionValueCompared = await _permissionService.GetByAliasAsync("uint64_file_read_power_needed");

            //Get file creation comparable own PermissionValue
            var permissionValuesComparedOwn =
                await _permissionService.GetByAliasAsync("uint64_file_read_own_power_needed");

            //Get file creation comparable system PermissionValue
            var permissionValueComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "uint64_file_read_power_needed_system",
                    cancellationToken);

            //Authorize file read (system PermissionValue)
            if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToAnyPermissionValue(user, permission,
                    permissionValueComparedSystem,
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (file.UserId == user.Id)
            {
                //Authorize file read own PermissionValue
                if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToUserPermission(user, permission,
                        permissionValuesComparedOwn, cancellationToken))
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }
            else
            {
                //Authorize file read PermissionValue
                if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToUserPermission(user, permission,
                        permissionValueCompared, cancellationToken))
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = file.Name,
                Inline = true,
            };
            _httpContext.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new FileReadResult
            {
                Data = file.Data,
                ContentType = file.ContentType
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Delete)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var file = await _fileService.GetByIdAsync(data.Id, cancellationToken);

            //Get file creation Permission that will be compared
            var permission = await _permissionService.GetByAliasAsync("uint64_file_modify_power");

            //Get file creation comparable PermissionValue
            var permissionValueCompared = await _permissionService.GetByAliasAsync("uint64_file_modify_power_needed");

            //Get file creation comparable own PermissionValue
            var permissionValuesComparedOwn =
                await _permissionService.GetByAliasAsync("uint64_file_modify_own_power_needed");

            //Get file creation comparable system PermissionValue
            var permissionValueComparedSystem =
                await _userToUserGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "uint64_file_modify_power_needed_system",
                    cancellationToken);

            //Authorize file read (system PermissionValue)
            if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToAnyPermissionValue(user, permission,
                    permissionValueComparedSystem,
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (file.UserId == user.Id)
            {
                //Authorize file read own PermissionValue
                if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToUserPermission(user, permission,
                        permissionValuesComparedOwn, cancellationToken))
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }
            else
            {
                //Authorize file read PermissionValue
                if (!await _userToUserGroupAdvancedService.AuthorizeUserPermissionToUserPermission(user, permission,
                        permissionValueCompared, cancellationToken))
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }

            await _fileService.Delete(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

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