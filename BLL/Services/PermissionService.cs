using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Castle.Core.Internal;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using IComparable = System.IComparable;
using ValueType = Common.Enums.ValueType;

namespace BLL.Services;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionService : IEntityServiceBase<Permission>
{
    Task<Permission> GetByAliasAndTypeAsync(string alias, PermissionType permissionType);

    Task<(int Total, IReadOnlyCollection<Permission> Items)> GetFilteredSortedPaged(
        FilterMatchModel filterMatchModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );

    // Task<(int Total, IReadOnlyCollection<Permission> Items)> GetBySubAliasAndTypeAsync(
    //     string subAlias,
    //     PermissionType permissionType,
    //     int page,
    //     int pageSize,
    //     CancellationToken cancellationToken = default
    // );
}

public class PermissionService : IPermissionService
{
    #region Fields

    private readonly ILogger<PermissionService> _logger;
    private readonly IPermissionRepository _permissionRepository;

    #endregion

    #region Ctor

    public PermissionService(ILogger<PermissionService> logger, IPermissionRepository permissionRepository)
    {
        _logger = logger;
        _permissionRepository = permissionRepository;
    }

    #endregion

    #region Methods

    public Task<Permission> Save(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public Task Delete(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public async Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _permissionRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<Permission> GetByAliasAndTypeAsync(string alias, PermissionType permissionType)
    {
        var entity =
            await _permissionRepository.SingleOrDefaultAsync(_ => _.Alias == alias && _.Type == permissionType);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByAliasAndTypeAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<(int Total, IReadOnlyCollection<Permission> Items)> GetFilteredSortedPaged(
        FilterMatchModel filterMatchModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var objectType = _permissionRepository.GetEntityType();

        string rawSql = "select * from \"" + _permissionRepository.GetTableName() + "\"";

        var rawSqlMatchParameters = new Dictionary<string, (string name, NpgsqlParameter parameter)>();

        void AddParameter<TValue>(PropertyInfo property, FilterMatchMode filterMatchMode, string sqlParameterName, TValue value)
        {
            switch (filterMatchMode)
            {
                case FilterMatchMode.None:
                    break;
                case FilterMatchMode.Equal:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " = ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.NotEqual:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " != ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.Less:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " < ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.LessOrEqual:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " <= ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.Greater:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " > ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.GreaterOrEqual:
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " >= ",
                        (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
                    break;
                case FilterMatchMode.StringContains:
                case FilterMatchMode.StringNotContains:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Match

        var sqlParameterCounter = 0;

        if (filterMatchModel.MatchRules.Any())
            rawSql += " WHERE ";

        foreach (var matchRule in filterMatchModel.MatchRules)
        {
            var property = objectType.GetProperty(matchRule.Key);

            if (property == null || property.GetCustomAttribute<AllowFilterMatchAttribute>(true) == null)
                throw new CustomException(Localize.Error.FilterMatchModelPropertyUnavailable);

            var sqlParameterCounterAsString = sqlParameterCounter.ToString();

            switch (matchRule.ValueType)
            {
                case ValueType.Unknown:
                    throw new CustomException(Localize.Error.FilterMatchModelItemValueTypeUnknown);
                case ValueType.Boolean:
                {
                    var value = BitConverter.ToBoolean(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Int8:
                {
                    var value = matchRule.Value[0];
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Int16:
                {
                    var value = BitConverter.ToInt16(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Int32:
                {
                    var value = BitConverter.ToInt32(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Int64:
                {
                    var value = BitConverter.ToInt64(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.UInt8:
                {
                    var value = matchRule.Value[0];
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.UInt16:
                {
                    var value = BitConverter.ToUInt16(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.UInt32:
                {
                    var value = BitConverter.ToUInt32(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.UInt64:
                {
                    var value = BitConverter.ToUInt64(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Float:
                {
                    var value = BitConverter.ToSingle(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Double:
                {
                    var value = BitConverter.ToDouble(matchRule.Value);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.Decimal:
                {
                    var value = new decimal(BitConverter.ToInt32(matchRule.Value),
                        BitConverter.ToInt32(matchRule.Value, sizeof(int)),
                        BitConverter.ToInt32(matchRule.Value, sizeof(int) * 2), matchRule.Value[15] == 0x80,
                        matchRule.Value[14]);
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                case ValueType.String:
                {
                    var value = Encoding.UTF8.GetString(matchRule.Value);
                    rawSqlMatchParameters.Add('"' + property.Name + '"' + " LIKE ",
                        (sqlParameterCounterAsString, new NpgsqlParameter<string>(sqlParameterCounterAsString, '%' + value + '%')));
                    break;
                }
                case ValueType.DateTime:
                {
                    var value = DateTime.FromBinary(BitConverter.ToInt64(matchRule.Value));
                    AddParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        rawSql += string.Join(" AND ", rawSqlMatchParameters.Select(_ => _.Key + '@' + _.Value.name));

        var npgSqlParameters = rawSqlMatchParameters.Select(_ => _.Value.parameter).ToArray() as object[];

        var query = _permissionRepository.FromSql(rawSql, npgSqlParameters);

        //Sort

        //TODO: Probs should use raw sql too
        foreach (var sortRule in filterSortModel.SortRules)
        {
            var property = objectType.GetProperty(sortRule.Key);

            if (property != null && property.GetCustomAttribute<AllowFilterSortAttribute>(true) != null)
            {
                query = sortRule.FilterSortMode switch
                {
                    FilterSortMode.Ascending => query.OrderBy(_ => property.GetValue(_)),
                    FilterSortMode.Descending => query.OrderByDescending(_ => property.GetValue(_)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        //Paginate

        var result = await query.ToArrayAsync(cancellationToken); //.GetPage(pageModel)
        
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFilteredSortedPaged),
                $"{result?.GetType().Name} {result?.Length}"));

        return (query.Count(), result);
    }

    // public async Task<(int Total, IReadOnlyCollection<Permission> Items)> GetBySubAliasAndTypeAsync(
    //     string subAlias,
    //     PermissionType permissionType,
    //     int page,
    //     int pageSize,
    //     CancellationToken cancellationToken = default
    // )
    // {
    //     var query = _permissionRepository.QueryMany(_ =>
    //         _.Alias.Contains(subAlias) && (_.Type == permissionType || permissionType == PermissionType.None));
    //
    //     var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellationToken);
    //
    //     _logger.Log(LogLevel.Information,
    //         Localize.Log.Method(GetType(), nameof(GetBySubAliasAndTypeAsync),
    //             $"{result?.GetType().Name} {result?.Length}"));
    //
    //     return (query.Count(), result);
    // }

    #endregion
}