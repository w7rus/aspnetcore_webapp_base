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
using Common.Exceptions;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Domain.Enums;
using DTO.Models.File;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using File = Domain.Entities.File;

namespace BLL.Services;

public interface IFileService : IEntityServiceBase<File>
{
    Task<File> Add(
        string fileName,
        byte[] data,
        AgeRating ageRating,
        Dictionary<string, string> metadata,
        Guid? userId,
        CancellationToken cancellationToken = new()
    );
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

    public async Task Save(File entity, CancellationToken cancellationToken = new())
    {
        _fileRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(File entity, CancellationToken cancellationToken = new())
    {
        var query = new NameValueCollection
        {
            {"FileName", entity.Name}
        };

        var queryString = string.Join("&",
            query.AllKeys.Select(_ => _ + "=" + HttpUtility.UrlEncode(query[_])));

        var httpClient = new HttpClient();
        var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
            _miscOptions.FileServer.Port, _miscOptions.FileServer.Path, "?" + queryString);
        var response = await httpClient.DeleteAsync(uriBuilder.ToString(), cancellationToken);

        httpClient.Dispose();
        if (!response.IsSuccessStatusCode)
            throw new CustomException();

        _fileRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<File> GetByIdAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var file = await _fileRepository.SingleOrDefaultAsync(_ => _.Id == id);

        var query = new NameValueCollection
        {
            {"FileName", file.Name}
        };

        var queryString = string.Join("&",
            query.AllKeys.Select(_ => _ + "=" + HttpUtility.UrlEncode(query[_])));

        var cdnServer = _miscOptions.CDNServers.FirstOrDefault() ?? throw new CustomException();

        var httpClient = new HttpClient();
        var uriBuilder = new UriBuilder(cdnServer.Scheme, cdnServer.Host,
            cdnServer.Port, cdnServer.Path, "?" + queryString);
        var response = await httpClient.GetAsync(uriBuilder.ToString(), cancellationToken);
        httpClient.Dispose();
        if (!response.IsSuccessStatusCode)
            throw new CustomException();
        var ms = new MemoryStream();
        await response.Content.CopyToAsync(ms, cancellationToken);
        file.Data = ms.ToArray();
        file.ContentType = response.Content.Headers.ContentType?.ToString();
        return file;
    }

    public async Task<File> Add(
        string fileName,
        byte[] data,
        AgeRating ageRating,
        Dictionary<string, string> metadata,
        Guid? userId,
        CancellationToken cancellationToken = new()
    )
    {
        var httpClient = new HttpClient();
        var uriBuilder = new UriBuilder(_miscOptions.FileServer.Scheme, _miscOptions.FileServer.Host,
            _miscOptions.FileServer.Port, _miscOptions.FileServer.Path);
        var response = await httpClient.PostAsync(uriBuilder.ToString(), new MultipartFormDataContent
        {
            {JsonContent.Create(new FileCDNCreate()), "data"},
            {new ByteArrayContent(data), "file", fileName}
        }, cancellationToken);

        httpClient.Dispose();
        if (!response.IsSuccessStatusCode)
            throw new CustomException();

        var fileCreateResult =
            JsonConvert.DeserializeObject<FileCDNCreateResult>(
                await response.Content.ReadAsStringAsync(cancellationToken));

        var entity = new File
        {
            Name = fileCreateResult?.FileName,
            Size = data.Length,
            AgeRating = ageRating,
            Metadata = metadata,
            UserId = userId,
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    #endregion
}