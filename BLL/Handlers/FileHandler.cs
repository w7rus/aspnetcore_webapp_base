using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly AppDbContext _appDbContext;

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
        IUserAdvancedService userAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        AppDbContext appDbContext
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
        _userAdvancedService = userAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContext = appDbContext;
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

            var rawSql = new AuthorizeModel
            {
                EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                EntityLeftId = $"'{user.Id.ToString()}'",
                EntityLeftPermissionAlias = "'g_file_a_create_o_file'",
                EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                EntityRightId = $"'{user.Id.ToString()}'",
                EntityRightPermissionAlias = "'g_file_a_create_o_file'",
                SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
            }.GetRawSql();

            //Authorize file create
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw(new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_file_a_create_o_file'",
                    EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                    EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityRightId = $"'{user.Id.ToString()}'",
                    EntityRightPermissionAlias = "'g_file_a_create_o_file'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                }.GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(File.AgeRating),
                        _appDbContext.Set<AuthorizeResult>()
                            .FromSqlRaw(new AuthorizeModel
                            {
                                EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                                EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                                EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                                EntityLeftId = $"'{user.Id.ToString()}'",
                                EntityLeftPermissionAlias = "'g_file_a_create_o_file.o_agerating_l_automapper'",
                                EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                                EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                                EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                                EntityRightId = $"'{user.Id.ToString()}'",
                                EntityRightPermissionAlias = "'g_file_a_create_o_file.o_agerating_l_automapper'",
                                SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                            }.GetRawSql()).ToList().SingleOrDefault()?.Result ?? false
                    }
                }
            };
            
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

            //Authorize file read
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw(new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_file_a_read_o_file'",
                    EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                    EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityRightId = $"'{file.UserId.ToString()}'",
                    EntityRightPermissionAlias = "'g_file_a_read_o_file'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                }.GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
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

            //Authorize file update
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw(new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_file_a_update_o_file'",
                    EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                    EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityRightId = $"'{file.UserId.ToString()}'",
                    EntityRightPermissionAlias = "'g_file_a_update_o_file'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                }.GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(File.AgeRating),
                        _appDbContext.Set<AuthorizeResult>()
                            .FromSqlRaw(new AuthorizeModel
                            {
                                EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                                EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                                EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                                EntityLeftId = $"'{user.Id.ToString()}'",
                                EntityLeftPermissionAlias = "'g_file_a_update_o_file.o_agerating_l_automapper'",
                                EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                                EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                                EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                                EntityRightId = $"'{file.UserId.ToString()}'",
                                EntityRightPermissionAlias = "'g_file_a_update_o_file.o_agerating_l_automapper'",
                                SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                            }.GetRawSql()).ToList().SingleOrDefault()?.Result ?? false
                    }
                }
            };
            
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

            //Authorize file delete
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw(new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_file_a_delete_o_file'",
                    EntityRightTableName = $"'{_userRepository.GetTableName()}'",
                    EntityRightGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityRightId = $"'{file.UserId.ToString()}'",
                    EntityRightPermissionAlias = "'g_file_a_delete_o_file'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                }.GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
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