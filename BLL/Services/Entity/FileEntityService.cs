using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using DTO.Models.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Services.Entity;

public interface IFileEntityService : IEntityServiceBase<File>
{
}

public class FileEntityService : IFileEntityService
{
    #region Ctor

    public FileEntityService(
        ILogger<FileEntityService> logger,
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

    #region Fields

    private readonly ILogger<FileEntityService> _logger;
    private readonly IFileRepository<File> _fileRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly MiscOptions _miscOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Methods

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

            var fileCdnCreateResult = await JsonSerializer.DeserializeAsync<FileCDNCreateResultDto>(
                                          await response.Content.ReadAsStreamAsync(cancellationToken),
                                          cancellationToken: cancellationToken) ??
                                      throw new CustomException(Localize.Error.ObjectDeserializationFailed);

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

    public async Task Delete(File entity, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
            _miscOptions.FileServer.Port, _miscOptions.FileServer.Path, "?" + await Utilities.ToHttpQueryString(
                new FileCDNDeleteDto
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

    public async Task<File> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.SingleOrDefaultAsync(_ => _.Id == id);

        var random = new Random();

        var cdnServer = _miscOptions.CdnServers.ElementAt(random.Next(0, _miscOptions.CdnServers.Count)) ??
                        throw new CustomException();

        var httpClient = _httpClientFactory.CreateClient();
        var uriBuilder = new UriBuilder(cdnServer.Scheme, cdnServer.Host,
            cdnServer.Port, cdnServer.Path, "?" + await Utilities.ToHttpQueryString(new FileCDNReadDto
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