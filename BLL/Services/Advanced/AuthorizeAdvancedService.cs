using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.Advanced;

public interface IAuthorizeAdvancedService
{
    bool Authorize(AuthorizeModel authorizeModel);
}

public class AuthorizeAdvancedService : IAuthorizeAdvancedService
{
    #region Fields

    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeEntityService _authorizeEntityService;

    #endregion

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
            
        var result = authorizeModelResult?.Result != null && authorizeModelResult.Result;
        
        //TODO: Add hangfire task to remove authorize cache each hour
        //TODO: Remove authorize cache when updating permissionValues

        return result;
    }

    #endregion
}