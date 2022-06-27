using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Maps;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DAL.Repository;
using DTO.Models.File;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using File = Domain.Entities.File;

namespace BLL.Handlers;

public interface IFileHandler
{
    Task<DTOResultBase> Create(
        FileCreateDto data,
        string fileNameOriginal,
        Stream stream,
        CancellationToken cancellationToken = default
    );

    Task<DTOResultBase> Read(FileReadDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Update(FileUpdateDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(FileDeleteDto data, CancellationToken cancellationToken = default);
}

public class FileHandler : HandlerBase, IFileHandler
{
    #region Ctor

    public FileHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IFileEntityService fileEntityService,
        IHttpContextAccessor httpContextAccessor,
        IUserEntityService userEntityService,
        IMapper mapper,
        IPermissionEntityService permissionEntityService,
        IPermissionValueEntityService permissionValueEntityService,
        IUserGroupEntityService userGroupEntityService,
        IUserAdvancedService userAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        AppDbContext appDbContext,
        IAuthorizeAdvancedService authorizeAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _fileEntityService = fileEntityService;
        _userEntityService = userEntityService;
        _mapper = mapper;
        _permissionEntityService = permissionEntityService;
        _permissionValueEntityService = permissionValueEntityService;
        _userGroupEntityService = userGroupEntityService;
        _userAdvancedService = userAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContext = appDbContext;
        _authorizeAdvancedService = authorizeAdvancedService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IFileEntityService _fileEntityService;
    private readonly HttpContext _httpContext;
    private readonly IUserEntityService _userEntityService;
    private readonly IMapper _mapper;
    private readonly IPermissionEntityService _permissionEntityService;
    private readonly IPermissionValueEntityService _permissionValueEntityService;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(
        FileCreateDto data,
        string fileNameOriginal,
        Stream stream,
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

            var fileInfo = new FileInfo(fileNameOriginal);
            var fileName = Guid.NewGuid() + fileInfo.Extension;

            //Authorize file create
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_file_a_create_o_file,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = user.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_file_a_create_o_file,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(File.AgeRating),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_file_a_create_o_file_o_agerating_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = user.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_file_a_create_o_file_o_agerating_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
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
            file.Stream = stream;
            file.Size = file.Stream.Length;
            file.Metadata = new Dictionary<string, string>();
            file.UserId = user.Id;

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create), $"Set additional data to {file.GetType().Name}"));

            file = await _fileEntityService.Save(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Create)));

            return _mapper.Map<FileCreateResultDto>(file);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Read(FileReadDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

            var file = await _fileEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (file is not {UserId: { }})
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.FileNotFound);

            //Authorize file read
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_file_a_read_o_file,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = file.UserId.Value,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_file_a_read_o_file,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var contentDisposition = new ContentDisposition
            {
                FileName = file.Name,
                Inline = true
            };
            _httpContext.Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new FileReadResultDto
            {
                FileStream = file.Stream,
                ContentType = file.ContentType,
                FileName = file.Name
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Update(FileUpdateDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

            var file = await _fileEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (file is not {UserId: { }})
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.FileNotFound);

            //Authorize file update
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_file_a_update_o_file,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = file.UserId.Value,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_file_a_update_o_file,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(File.AgeRating),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_file_a_update_o_file_o_agerating_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = file.UserId.Value,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_file_a_update_o_file_o_agerating_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"Id\"'"
                        })
                    }
                }
            };

            //Map with conditional authorization. Mapping configuration profile is located at BLL.Maps.AutoMapperProfile
            _mapper.Map(data, file,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {file.GetType().Name}"));

            await _fileEntityService.Save(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return _mapper.Map<FileUpdateResultDto>(file);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Delete(FileDeleteDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

            var file = await _fileEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (file is not {UserId: { }})
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.FileNotFound);

            //Authorize file delete
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_file_a_delete_o_file,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = file.UserId.Value,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_file_a_delete_o_file,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _fileEntityService.Delete(file, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}