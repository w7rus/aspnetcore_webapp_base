using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BLL.Services.Base;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using File = Domain.Entities.File;

namespace BLL.Services;

public interface IFileService : IEntityServiceBase<File>
{
}

public class FileService : IFileService
{
    #region Fields

    private readonly ILogger<FileService> _logger;
    private readonly IFileRepository<File> _fileRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly MiscOptions _miscOptions;

    #endregion

    #region Ctor

    public FileService(
        ILogger<FileService> logger,
        IFileRepository<File> fileRepository,
        IAppDbContextAction appDbContextAction,
        IOptions<MiscOptions> miscOptions
    )
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _appDbContextAction = appDbContextAction;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Saves entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<File> Save(File entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity.GetType().Name} {entity.Id}"));

        if (entity.IsNew())
        {
            var httpClient = new HttpClient();
            var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
                _miscOptions.FileServer.Port, _miscOptions.FileServer.Path);
            var response = await httpClient.PostAsync(uriBuilder.ToString(), new MultipartFormDataContent
            {
                {JsonContent.Create(new FileCDNCreate()), "data"},
                {new ByteArrayContent(entity.Data), "file", entity.Name}
            }, cancellationToken);

            httpClient.Dispose();
            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                    Localize.Error.ResponseStatusCodeUnsuccessful);

            var fileCdnCreateResult =
                JsonConvert.DeserializeObject<FileCDNCreateResult>(
                    await response.Content.ReadAsStringAsync(cancellationToken));

            if (fileCdnCreateResult == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.JsonConvert,
                    Localize.Error.ObjectDeserializationFailed);

            entity.Name = fileCdnCreateResult.FileName;
        }
        else
        {
            var file = await _fileRepository.SingleOrDefaultAsync(_ => _.Id == entity.Id);

            if (file.Name != entity.Name)
                throw new CustomException(Localize.Error.FileSaveFailedNameChangeNotAllowedForExisting);
        }

        _fileRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Deletes entity & Sends response to delete file on FS
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="CustomException"></exception>
    public async Task Delete(File entity, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
            _miscOptions.FileServer.Port, _miscOptions.FileServer.Path, "?" + await Utilities.ToHttpQueryString(
                new FileCDNDelete
                {
                    FileName = entity.Name
                }));
        var response = await httpClient.DeleteAsync(uriBuilder.ToString(), cancellationToken);

        httpClient.Dispose();
        if (!response.IsSuccessStatusCode)
            throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                Localize.Error.ResponseStatusCodeUnsuccessful);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity.GetType().Name} {entity.Id}"));

        _fileRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Gets entity by PK & Retrieves file from CDN or FS
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<File> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.SingleOrDefaultAsync(_ => _.Id == id);

        var cdnServer = _miscOptions.CDNServers.FirstOrDefault() ?? throw new CustomException();

        var httpClient = new HttpClient();
        var uriBuilder = new UriBuilder(cdnServer.Scheme, cdnServer.Host,
            cdnServer.Port, cdnServer.Path, "?" + await Utilities.ToHttpQueryString(new FileCDNRead
            {
                FileName = file.Name
            }));
        var response = await httpClient.GetAsync(uriBuilder.ToString(), cancellationToken);

        httpClient.Dispose();
        if (!response.IsSuccessStatusCode)
            throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                Localize.Error.ResponseStatusCodeUnsuccessful);

        file.Data = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        file.ContentType = response.Content.Headers.ContentType?.ToString();

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{file.GetType().Name} {file.Id}"));

        return file;
    }

    #endregion
}