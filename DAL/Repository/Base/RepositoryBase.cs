using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Extensions;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using ValueType = Common.Enums.ValueType;

namespace DAL.Repository.Base
{
    public interface IRepositoryBase<TEntity, in TKey> where TEntity : EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        void Add(TEntity entity);
        void Add(IEnumerable<TEntity> entities);
        Task AddAsync(IEnumerable<TEntity> entities);
        void Save(TEntity entity);
        void Save(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<TEntity> GetByIdOrDefaultAsync(TKey id);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TResult> QueryMany<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector
        );

        IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector);
        IQueryable<TEntity> QueryAll();
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        Type GetEntityType();
        string GetTableName();

        public (int total, IQueryable<TEntity> entities) GetFilteredSortedPaged(
            FilterExpressionModel filterExpressionModel,
            FilterSortModel filterSortModel,
            PageModel pageModel
        );
    }

    public abstract class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
        where TEntity : EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        private AppDbContext AppDbContext { get; }
        private DbSet<TEntity> DbSet { get; }

        protected RepositoryBase(AppDbContext appDbContext)
        {
            AppDbContext = appDbContext;
            DbSet = AppDbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public void Save(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public void Save(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await DbSet.SingleAsync(_ => _.Id.Equals(id));
        }

        public async Task<TEntity> GetByIdOrDefaultAsync(TKey id)
        {
            return await DbSet.SingleOrDefaultAsync(_ => _.Id.Equals(id));
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.SingleAsync(predicate);
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.SingleOrDefaultAsync(predicate);
        }

        public IQueryable<TResult> QueryMany<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector
        )
        {
            return DbSet.Where(predicate).Select(selector);
        }

        public IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return DbSet.Select(selector);
        }

        public IQueryable<TEntity> QueryAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
        {
            return DbSet.FromSqlRaw(sql, parameters);
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }

        public string GetTableName()
        {
            var model = AppDbContext.Model;
            var entityTypes = model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == typeof(TEntity));
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            return tableNameAnnotation.Value?.ToString();
        }

        //TODO: Complex: Pagination won't go with items that are permission restricted. Create a stored procedure after solving TODO in EntityPermissionValueBase
        //TODO: Easier: Code Filter-Sort stage first, then query each item and authorize it, then use Filter-Sort query and select specific rows (Skip-Take) and finally paginate result query
        public (int total, IQueryable<TEntity> entities) GetFilteredSortedPaged(
            FilterExpressionModel filterExpressionModel,
            FilterSortModel filterSortModel,
            PageModel pageModel
        )
        {
            var providerName = AppDbContext.Database.ProviderName;
            if (providerName != Consts.NpgSqlEntityFrameworkCorePostgreSQLProviderName)
                throw new CustomException(Localize.Error.DbProviderNotSupported);

            var entityType = GetEntityType();

            var rawSql = "SELECT * FROM " + '"' + GetTableName() + '"';

            var rawSqlParameters = new List<NpgsqlParameter>();

            void AddMatchParameter<TValue>(
                MemberInfo property,
                FilterMatchOperation filterMatchOperation,
                string sqlParameterName,
                TValue value
            )
            {
                var filterMatchOperationAsString = filterMatchOperation switch
                {
                    FilterMatchOperation.None => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Equal => " = ",
                    FilterMatchOperation.NotEqual => " != ",
                    FilterMatchOperation.Less => " < ",
                    FilterMatchOperation.LessOrEqual => " <= ",
                    FilterMatchOperation.Greater => " > ",
                    FilterMatchOperation.GreaterOrEqual => " >= ",
                    FilterMatchOperation.Contains => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.StartsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.EndsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSql += '"' + property.Name + '"' + filterMatchOperationAsString + '@' + sqlParameterName;

                rawSqlParameters.Add(new NpgsqlParameter<TValue>(sqlParameterName, value));
            }

            void AddMatchParameterEquatable<TValue>(
                MemberInfo property,
                FilterMatchOperation filterMatchOperation,
                string sqlParameterName,
                TValue value
            )
            {
                var filterMatchOperationAsString = filterMatchOperation switch
                {
                    FilterMatchOperation.None => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Equal => " = ",
                    FilterMatchOperation.NotEqual => " != ",
                    FilterMatchOperation.Less => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.LessOrEqual => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Greater => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.GreaterOrEqual => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Contains => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.StartsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.EndsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSql += '"' + property.Name + '"' + filterMatchOperationAsString + '@' + sqlParameterName;

                rawSqlParameters.Add(new NpgsqlParameter<TValue>(sqlParameterName, value));
            }

            void AddMatchParameterString(
                MemberInfo property,
                FilterMatchOperation filterMatchOperation,
                string sqlParameterName,
                string value
            )
            {
                var filterMatchOperationAsString = filterMatchOperation switch
                {
                    FilterMatchOperation.None => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Equal => " = ",
                    FilterMatchOperation.NotEqual => " != ",
                    FilterMatchOperation.Less => " < ",
                    FilterMatchOperation.LessOrEqual => " <= ",
                    FilterMatchOperation.Greater => " > ",
                    FilterMatchOperation.GreaterOrEqual => " >= ",
                    FilterMatchOperation.Contains => " LIKE ",
                    FilterMatchOperation.StartsWith => " LIKE ",
                    FilterMatchOperation.EndsWith => " LIKE ",
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSql += '"' + property.Name + '"' + filterMatchOperationAsString + '@' + sqlParameterName;

                rawSqlParameters.Add(new NpgsqlParameter<string>(sqlParameterName,
                    (filterMatchOperation is FilterMatchOperation.Contains or FilterMatchOperation.StartsWith
                        ? '%'
                        : string.Empty) + value +
                    (filterMatchOperation is FilterMatchOperation.Contains or FilterMatchOperation.EndsWith
                        ? '%'
                        : string.Empty)));
            }

            void AddMatchParameterDateTime(
                MemberInfo property,
                FilterMatchOperation filterMatchOperation,
                string sqlParameterName,
                DateTime value
            )
            {
                var filterMatchOperationAsString = filterMatchOperation switch
                {
                    FilterMatchOperation.None => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.Equal => " = ",
                    FilterMatchOperation.NotEqual => " != ",
                    FilterMatchOperation.Less => " < ",
                    FilterMatchOperation.LessOrEqual => " <= ",
                    FilterMatchOperation.Greater => " > ",
                    FilterMatchOperation.GreaterOrEqual => " >= ",
                    FilterMatchOperation.Contains => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.StartsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    FilterMatchOperation.EndsWith => throw new CustomException(Localize.Error
                        .FilterMatchModelValueTypeNotCompatible),
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSql += '"' + property.Name + '"' + filterMatchOperationAsString + '@' + sqlParameterName;

                rawSqlParameters.Add(new NpgsqlParameter<string>(sqlParameterName,
                    "to_timestamp(" + value.ToString("yyyy-MM-dd HH:mm:ss zzz") +
                    "\'YYYY-MM-DD HH24:MI:SS TZH:TZM\')"));
            }

            var sqlParameterCounter = 0;

            //Match

            if (filterExpressionModel != null)
            {
                if (filterExpressionModel.Items.Any())
                    rawSql += " WHERE ";

                var stack = new Stack<FilterExpressionModelItemStackItem>();
                stack.Push(new FilterExpressionModelItemStackItem()
                {
                    Items = filterExpressionModel.Items,
                    Index = 0
                });
                if (filterExpressionModel.ExpressionLogicalOperation > ExpressionLogicalOperation.Not)
                    throw new CustomException(Localize.Error
                        .FilterMatchModelItemFirstExpressionLogicalOperationNoneNotOnly);
                rawSql += '(';

                while (stack.Any())
                {
                    var current = stack.Peek();

                    for (; current.Index < current.Items.Count; current.Index++)
                    {
                        var scopeItem = current.Items[current.Index];

                        switch (current.Index)
                        {
                            case 0 when scopeItem.ExpressionLogicalOperation > ExpressionLogicalOperation.Not:
                                throw new CustomException(Localize.Error
                                    .FilterMatchModelItemFirstExpressionLogicalOperationNoneNotOnly);
                            case > 0 when scopeItem.ExpressionLogicalOperation <= ExpressionLogicalOperation.Not:
                                throw new CustomException(Localize.Error
                                    .FilterMatchModelItemNotFirstExpressionLogicalOperationAndOrOnly);
                        }

                        switch (scopeItem.ExpressionLogicalOperation)
                        {
                            case ExpressionLogicalOperation.None:
                                break;
                            case ExpressionLogicalOperation.Not:
                                rawSql += "NOT ";
                                break;
                            case ExpressionLogicalOperation.And:
                                rawSql += " AND ";
                                break;
                            case ExpressionLogicalOperation.Or:
                                rawSql += " OR ";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (scopeItem is FilterExpressionModelItemScope itemScope)
                        {
                            current.Index++;
                            stack.Push(new FilterExpressionModelItemStackItem()
                            {
                                Items = itemScope.Items,
                                Index = 0
                            });
                            rawSql += '(';
                            break;
                        }

                        if (scopeItem is FilterExpressionModelItemExpression itemExpression)
                        {
                            var property = entityType.GetProperty(itemExpression.Key);

                            if (property == null ||
                                property.GetCustomAttribute<AllowFilterExpressionAttribute>(true) == null)
                                throw new CustomException(Localize.Error.FilterMatchModelPropertyNotFoundOrUnavailable);

                            var sqlParameterCounterAsString = sqlParameterCounter.ToString();

                            switch (property.GetValueType())
                            {
                                case ValueType.None:
                                case ValueType.Unknown:
                                    throw new CustomException(Localize.Error
                                        .FilterMatchModelItemExpressionValueTypeNotSupported);
                                case ValueType.Boolean:
                                {
                                    var value = BitConverter.ToBoolean(itemExpression.Value);
                                    AddMatchParameterEquatable(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Int8:
                                {
                                    var value = (sbyte) itemExpression.Value[0];
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Int16:
                                {
                                    var value = BitConverter.ToInt16(itemExpression.Value);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Int32:
                                {
                                    var value = BitConverter.ToInt32(itemExpression.Value);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Int64:
                                {
                                    var value = BitConverter.ToInt64(itemExpression.Value);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.UInt8:
                                {
                                    throw new CustomException(Localize.Error
                                        .FilterMatchModelItemExpressionValueTypeNotSupported);
                                    //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                                    // var value = matchRule.Value[0];
                                    // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                                    // break;
                                }
                                case ValueType.UInt16:
                                {
                                    throw new CustomException(Localize.Error
                                        .FilterMatchModelItemExpressionValueTypeNotSupported);
                                    //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                                    // var value = BitConverter.ToUInt16(matchRule.Value);
                                    // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                                    // break;
                                }
                                case ValueType.UInt32:
                                {
                                    throw new CustomException(Localize.Error
                                        .FilterMatchModelItemExpressionValueTypeNotSupported);
                                    //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                                    // var value = BitConverter.ToUInt32(matchRule.Value);
                                    // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                                    // break;
                                }
                                case ValueType.UInt64:
                                {
                                    throw new CustomException(Localize.Error
                                        .FilterMatchModelItemExpressionValueTypeNotSupported);
                                    //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                                    // var value = BitConverter.ToUInt64(matchRule.Value);
                                    // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                                    // break;
                                }
                                case ValueType.Float:
                                {
                                    var value = BitConverter.ToSingle(itemExpression.Value);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Double:
                                {
                                    var value = BitConverter.ToDouble(itemExpression.Value);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Decimal:
                                {
                                    var value = new decimal(BitConverter.ToInt32(itemExpression.Value),
                                        BitConverter.ToInt32(itemExpression.Value, sizeof(int)),
                                        BitConverter.ToInt32(itemExpression.Value, sizeof(int) * 2),
                                        itemExpression.Value[15] == 0x80,
                                        itemExpression.Value[14]);
                                    AddMatchParameter(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.String:
                                {
                                    var value = Encoding.UTF8.GetString(itemExpression.Value);
                                    AddMatchParameterString(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.DateTime:
                                {
                                    var value = DateTime.FromBinary(BitConverter.ToInt64(itemExpression.Value));
                                    AddMatchParameterDateTime(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                case ValueType.Guid:
                                {
                                    if (!Guid.TryParse(Encoding.UTF8.GetString(itemExpression.Value), out var value))
                                        throw new CustomException(Localize.Error
                                            .FilterMatchModelItemExpressionValueFailedToParseGuid);
                                    AddMatchParameterEquatable(property, itemExpression.FilterMatchOperation,
                                        sqlParameterCounterAsString, value);
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            sqlParameterCounter++;
                        }
                    }

                    current = stack.Peek();

                    if (current.Index < current.Items.Count) continue;

                    stack.Pop();
                    rawSql += ')';
                }
            }

            //Sort

            if (filterSortModel != null)
            {
                var rawSqlSortParameters = new List<string>();
                if (filterSortModel.SortRules.Any())
                    rawSql += " ORDER BY ";

                foreach (var sortRule in filterSortModel.SortRules)
                {
                    var property = entityType.GetProperty(sortRule.Key);

                    if (property == null || property.GetCustomAttribute<AllowFilterSortAttribute>(true) == null)
                        throw new CustomException(Localize.Error.FilterSortModelPropertyNotFoundOrUnavailable);

                    switch (sortRule.FilterSortMode)
                    {
                        case FilterSortMode.Ascending:
                            rawSqlSortParameters.Add('"' + property.Name + "\" ASC");
                            break;
                        case FilterSortMode.Descending:
                            rawSqlSortParameters.Add('"' + property.Name + "\" DESC");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                rawSql += string.Join(", ", rawSqlSortParameters);
            }

            // ReSharper disable once CoVariantArrayConversion
            var query = FromSql(rawSql, rawSqlParameters.ToArray());

            var total = query.Count();

            if (pageModel != null)
            {
                query = query.GetPage(pageModel);
            }

            return (total, query);
        }
    }
}