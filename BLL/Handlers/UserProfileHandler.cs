using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using Common.Models.Base;
using DAL.Data;
using DTO.Models.UserProfile;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserProfileHandler
{
    Task<IDtoResultBase> Create(UserProfileCreateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Read(UserProfileReadDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Update(UserProfileUpdateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Delete(UserProfileDeleteDto data, CancellationToken cancellationToken = default);
}

public class UserProfileHandler : HandlerBase, IUserProfileHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IMapper _mapper;

    #endregion

    #region Ctor

    public UserProfileHandler(ILogger<HandlerBase> logger, IAppDbContextAction appDbContextAction, IMapper mapper)
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _mapper = mapper;
    }

    #endregion

    #region Methods

    public async Task<IDtoResultBase> Create(UserProfileCreateDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IDtoResultBase> Read(UserProfileReadDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IDtoResultBase> Update(UserProfileUpdateDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IDtoResultBase> Delete(UserProfileDeleteDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion
}