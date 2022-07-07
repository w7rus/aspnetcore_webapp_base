using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using Common.Models.Base;
using DAL.Data;
using DTO.Models.User;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers.User;

public interface IUserHandler
{
    Task<IDtoResultBase> Read(UserReadDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> ReadCollection(
        UserReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> Update(UserUpdateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Delete(UserDeleteDto data, CancellationToken cancellationToken = default);
}

public class UserHandler : HandlerBase, IUserHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IMapper _mapper;

    #endregion

    #region Ctor

    public UserHandler(ILogger<HandlerBase> logger, IAppDbContextAction appDbContextAction, IMapper mapper)
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _mapper = mapper;
    }

    #endregion

    #region Methods

    public async Task<IDtoResultBase> Read(UserReadDto data, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IDtoResultBase> ReadCollection(
        UserReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        throw new System.NotImplementedException();
    }

    public async Task<IDtoResultBase> Update(UserUpdateDto data, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IDtoResultBase> Delete(UserDeleteDto data, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}