using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Maps;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using File = Domain.Entities.File;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(FileCreate data, IFormFile formFile, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(FileRead data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Update(FileUpdate data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(FileDelete data, CancellationToken cancellationToken = default);
}

public class FileHandler : HandlerBase, IFileHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IFileService _fileService;
    private readonly HttpContext _httpContext;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly IPermissionValueService _permissionValueService;
    private readonly IUserGroupService _userGroupService;
    private readonly IUserGroupAdvancedService _userGroupAdvancedService;
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
        IPermissionValueService permissionValueService,
        IUserGroupService userGroupService,
        IUserGroupAdvancedService userGroupAdvancedService,
        IUserAdvancedService userAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _fileService = fileService;
        _userService = userService;
        _mapper = mapper;
        _permissionService = permissionService;
        _permissionValueService = permissionValueService;
        _userGroupService = userGroupService;
        _userGroupAdvancedService = userGroupAdvancedService;
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
            var permission =
                await _permissionService.GetByAliasAndTypeAsync("g_file_a_create_o_file", PermissionType.Value);

            //Get file creation comparable system PermissionValue
            var permissionValueComparedSystem =
                await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                    "g_file_a_create_o_file",
                    cancellationToken);

            //Authorize file creation
            if (!await _userGroupAdvancedService.AuthorizePermissionToPermissionValue(user, permission,
                    permissionValueComparedSystem,
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData(
                user,
                null,
                new Dictionary<string, AutoMapperModelFieldAuthorizeData>
                {
                    {
                        nameof(File.AgeRating),
                        new AutoMapperModelFieldAuthorizeData
                        {
                            PermissionComparable =
                                await _permissionService.GetByAliasAndTypeAsync(
                                    "g_file_a_create_o_file.o_agerating_l_automapper", PermissionType.Value),
                            PermissionCompared = null,
                            PermissionValueSystemCompared =
                                await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                                    "g_file_a_create_o_file.o_agerating_l_automapper", cancellationToken),
                            CustomValueCompared = null
                        }
                    }
                });

            //Map with conditional authorization. Mapping configuration profile is located at BLL.Maps.AutoMapperProfile
            var file = _mapper.Map<File>(data,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

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

            return _mapper.Map<FileCreateResult>(file);
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

            var userPermissionComparable =
                await _permissionService.GetByAliasAndTypeAsync("g_file_a_read_o_file", PermissionType.Value);

            //Authorize file read
            if (!await _userGroupAdvancedService.AuthorizePermissionToPermissionValue(user,
                    userPermissionComparable,
                    await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                        "g_file_a_read_o_file",
                        cancellationToken),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (!await _userGroupAdvancedService.AuthorizePermissionToPermission(user,
                    userPermissionComparable, file.User,
                    await _permissionService.GetByAliasAndTypeAsync("g_file_a_read_o_file",
                        file.UserId == user.Id ? PermissionType.ValueNeededOwner : PermissionType.ValueNeededOthers),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

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

    public async Task<DTOResultBase> Update(FileUpdate data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Update)));

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

            var userPermissionComparable =
                await _permissionService.GetByAliasAndTypeAsync("g_file_a_update_o_file", PermissionType.Value);

            //Authorize file update
            if (!await _userGroupAdvancedService.AuthorizePermissionToPermissionValue(user,
                    userPermissionComparable,
                    await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                        "g_file_a_update_o_file",
                        cancellationToken),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (!await _userGroupAdvancedService.AuthorizePermissionToPermission(user,
                    userPermissionComparable, file.User,
                    await _permissionService.GetByAliasAndTypeAsync("g_file_a_update_o_file",
                        file.UserId == user.Id ? PermissionType.ValueNeededOwner : PermissionType.ValueNeededOthers),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData(
                user,
                file.User,
                new Dictionary<string, AutoMapperModelFieldAuthorizeData>
                {
                    {
                        nameof(File.AgeRating),
                        new AutoMapperModelFieldAuthorizeData
                        {
                            PermissionComparable =
                                await _permissionService.GetByAliasAndTypeAsync(
                                    "g_file_a_update_o_file.o_agerating_l_automapper", PermissionType.Value),
                            PermissionCompared = await _permissionService.GetByAliasAndTypeAsync(
                                "g_file_a_update_o_file.o_agerating_l_automapper",
                                file.UserId == user.Id
                                    ? PermissionType.ValueNeededOwner
                                    : PermissionType.ValueNeededOthers),
                            PermissionValueSystemCompared =
                                await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                                    "g_file_a_update_o_file.o_agerating_l_automapper", cancellationToken),
                            CustomValueCompared = null
                        }
                    }
                });

            //Map with conditional authorization. Mapping configuration profile is located at BLL.Maps.AutoMapperProfile
            _mapper.Map(data, file,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {file.GetType().Name}"));

            await _fileService.Save(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return _mapper.Map<FileUpdateResult>(file);
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
            var userPermissionComparable =
                await _permissionService.GetByAliasAndTypeAsync("g_file_a_delete_o_file", PermissionType.Value);

            //Authorize file delete
            if (!await _userGroupAdvancedService.AuthorizePermissionToPermissionValue(user,
                    userPermissionComparable,
                    await _userGroupAdvancedService.GetSystemPermissionValueByAlias(
                        "g_file_a_delete_o_file",
                        cancellationToken),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (!await _userGroupAdvancedService.AuthorizePermissionToPermission(user,
                    userPermissionComparable, file.User,
                    await _permissionService.GetByAliasAndTypeAsync("g_file_a_delete_o_file",
                        file.UserId == user.Id ? PermissionType.ValueNeededOwner : PermissionType.ValueNeededOthers),
                    cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _fileService.Delete(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

            return new FileDeleteResult();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}