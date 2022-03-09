using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository.Edition_Shop;
using Domain.Entities.Edition_Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Edition_Shop;

public interface IOperatingTimeService : IEntityServiceBase<OperatingTime>
{
}

public class OperatingTimeService : IOperatingTimeService
{
    #region Fields

    private readonly ILogger<OperatingTimeService> _logger;
    private readonly IOperatingTimeRepository _operatingTimeRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public OperatingTimeService(
        ILogger<OperatingTimeService> logger,
        IOperatingTimeRepository operatingTimeRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _operatingTimeRepository = operatingTimeRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(OperatingTime entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(OperatingTime entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OperatingTime> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OperatingTime> Create(OperatingTime entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}