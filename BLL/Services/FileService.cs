using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
using File = Domain.Entities.File;
using HttpMethod = System.Net.Http.HttpMethod;

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
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Ctor

    public FileService(
        ILogger<FileService> logger,
        IFileRepository<File> fileRepository,
        IAppDbContextAction appDbContextAction,
        IOptions<MiscOptions> miscOptions,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _appDbContextAction = appDbContextAction;
        _httpClientFactory = httpClientFactory;
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
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        if (entity.IsNew())
        {
            var httpClient = _httpClientFactory.CreateClient();
            var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
                _miscOptions.FileServer.Port, _miscOptions.FileServer.Path);
            var response = await httpClient.PostAsync(uriBuilder.ToString(), new MultipartFormDataContent
            {
                // {JsonContent.Create(new FileCDNCreate()), "data"},
                {new StreamContent(entity.Stream), "file", entity.Name}
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                    Localize.Error.ResponseStatusCodeUnsuccessful);

            var fileCdnCreateResult = (await JsonSerializer.DeserializeAsync(
                                           await response.Content.ReadAsStreamAsync(cancellationToken),
                                           typeof(FileCDNCreateResult),
                                           cancellationToken: cancellationToken) ??
                                       throw new CustomException(Localize.Error.ObjectDeserializationFailed)) as
                                      FileCDNCreateResult ??
                                      throw new CustomException(Localize.Error.ObjectCastFailed);

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
        var httpClient = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
            _miscOptions.FileServer.Port, _miscOptions.FileServer.Path, "?" + await Utilities.ToHttpQueryString(
                new FileCDNDelete
                {
                    FileName = entity.Name
                }));
        var response = await httpClient.DeleteAsync(uriBuilder.ToString(), cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                Localize.Error.ResponseStatusCodeUnsuccessful);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

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

        var random = new Random();

        var cdnServer = _miscOptions.CDNServers.ElementAt(random.Next(0, _miscOptions.CDNServers.Count)) ??
                        throw new CustomException();

        var httpClient = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(cdnServer.Scheme, cdnServer.Host,
            cdnServer.Port, cdnServer.Path, "?" + await Utilities.ToHttpQueryString(new FileCDNRead
            {
                FileName = file.Name
            }));

        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString()),
            HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpResponseException((int) response.StatusCode, ErrorType.HttpClient,
                Localize.Error.ResponseStatusCodeUnsuccessful);

        file.Stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        file.ContentType = response.Content.Headers.ContentType?.ToString();

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{file?.GetType().Name} {file?.Id}"));

        return file;
    }

    #endregion
}