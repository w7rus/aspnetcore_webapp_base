using System.Linq;
using BLL.Services.Entity;
using Common.Models;
using DAL.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.Advanced;

public interface IAuthorizeAdvancedService
{
    bool Authorize(AuthorizeModel authorizeModel);
}

public class AuthorizeAdvancedService : IAuthorizeAdvancedService
{
    #region Ctor

    public AuthorizeAdvancedService(AppDbContext appDbContext, IAuthorizeEntityService authorizeEntityService)
    {
        _appDbContext = appDbContext;
        _authorizeEntityService = authorizeEntityService;
    }

    #endregion

    #region Methods

    public bool Authorize(AuthorizeModel authorizeModel)
    {
        var sql = authorizeModel.GetRawSqlAuthorizeResult();

        var authorizeModelResult = _appDbContext.Set<AuthorizeResult>()
            .FromSqlRaw(sql).ToList().SingleOrDefault();

        return authorizeModelResult?.Result != null && authorizeModelResult.Result;
    }

    #endregion

    #region Fields

    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeEntityService _authorizeEntityService;

    #endregion
}