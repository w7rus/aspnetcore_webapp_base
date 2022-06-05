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
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            FilterMatchModel filterMatchModel,
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

        public (int total, IQueryable<TEntity> entities) GetFilteredSortedPaged(
            FilterMatchModel filterMatchModel,
            FilterSortModel filterSortModel,
            PageModel pageModel
        )
        {
            var providerName = AppDbContext.Database.ProviderName;
            if (providerName != Consts.NpgSqlEntityFrameworkCorePostgreSQLProviderName)
                throw new CustomException(Localize.Error.DbProviderNotSupported);

            var entityType = GetEntityType();
            
            var rawSql = "SELECT * FROM " + '"' + GetTableName() + '"';
            
            var rawSqlMatchParameters = new Dictionary<string, (string name, NpgsqlParameter parameter)>();
            
            void AddMatchParameter<TValue>(PropertyInfo property, FilterMatchMode filterMatchMode, string sqlParameterName, TValue value)
            {
                var filterMatchOp = filterMatchMode switch
                {
                    FilterMatchMode.None => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.Equal => " = ",
                    FilterMatchMode.NotEqual => " != ",
                    FilterMatchMode.Less => " < ",
                    FilterMatchMode.LessOrEqual => " <= ",
                    FilterMatchMode.Greater => " > ",
                    FilterMatchMode.GreaterOrEqual => " >= ",
                    FilterMatchMode.StringContains => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.StringNotContains => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSqlMatchParameters.Add('"' + property.Name + '"' + filterMatchOp,
                    (sqlParameterName, new NpgsqlParameter<TValue>(sqlParameterName, value)));
            }
            void AddMatchParameterString(PropertyInfo property, FilterMatchMode filterMatchMode, string sqlParameterName, string value)
            {
                var filterMatchOp = filterMatchMode switch
                {
                    FilterMatchMode.None => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.Equal => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.NotEqual => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.Less => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.LessOrEqual => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.Greater => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.GreaterOrEqual => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.StringContains => " LIKE ",
                    FilterMatchMode.StringNotContains => " NOT LIKE ",
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSqlMatchParameters.Add('"' + property.Name + '"' + filterMatchOp,
                    (sqlParameterName, new NpgsqlParameter<string>(sqlParameterName, '%' + value + '%')));
            }
            void AddMatchParameterDateTime(PropertyInfo property, FilterMatchMode filterMatchMode, string sqlParameterName, DateTime value)
            {
                var filterMatchOp = filterMatchMode switch
                {
                    FilterMatchMode.None => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.Equal => " = ",
                    FilterMatchMode.NotEqual => " != ",
                    FilterMatchMode.Less => " < ",
                    FilterMatchMode.LessOrEqual => " <= ",
                    FilterMatchMode.Greater => " > ",
                    FilterMatchMode.GreaterOrEqual => " >= ",
                    FilterMatchMode.StringContains => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    FilterMatchMode.StringNotContains => throw new CustomException(Localize.Error.FilterMatchModelValueTypeNotSuitable),
                    _ => throw new ArgumentOutOfRangeException()
                };

                rawSqlMatchParameters.Add('"' + property.Name + '"' + filterMatchOp,
                    (sqlParameterName, new NpgsqlParameter<string>(sqlParameterName, "to_timestamp(" + value.ToString("yyyy-MM-dd HH:mm:ss zzz") + "\'YYYY-MM-DD HH24:MI:SS TZH:TZM\')")));
            }
            
            var sqlParameterCounter = 0;
            
            //Match
            
            if (filterMatchModel.MatchRules.Any())
                rawSql += " WHERE ";
            
            foreach (var matchRule in filterMatchModel.MatchRules)
            {
                var property = entityType.GetProperty(matchRule.Key);

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
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Int8:
                    {
                        var value = matchRule.Value[0];
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Int16:
                    {
                        var value = BitConverter.ToInt16(matchRule.Value);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Int32:
                    {
                        var value = BitConverter.ToInt32(matchRule.Value);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Int64:
                    {
                        var value = BitConverter.ToInt64(matchRule.Value);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.UInt8:
                    {
                        throw new CustomException(Localize.Error.FilterMatchModelItemValueTypeNotSupported);
                        //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                        // var value = matchRule.Value[0];
                        // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        // break;
                    }
                    case ValueType.UInt16:
                    {
                        throw new CustomException(Localize.Error.FilterMatchModelItemValueTypeNotSupported);
                        //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                        // var value = BitConverter.ToUInt16(matchRule.Value);
                        // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        // break;
                    }
                    case ValueType.UInt32:
                    {
                        throw new CustomException(Localize.Error.FilterMatchModelItemValueTypeNotSupported);
                        //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                        // var value = BitConverter.ToUInt32(matchRule.Value);
                        // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        // break;
                    }
                    case ValueType.UInt64:
                    {
                        throw new CustomException(Localize.Error.FilterMatchModelItemValueTypeNotSupported);
                        //TODO: PostgreSql does not support unsigned value types, either create mappings or convert to BigInteger or Decimal?
                        // var value = BitConverter.ToUInt64(matchRule.Value);
                        // AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        // break;
                    }
                    case ValueType.Float:
                    {
                        var value = BitConverter.ToSingle(matchRule.Value);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Double:
                    {
                        var value = BitConverter.ToDouble(matchRule.Value);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.Decimal:
                    {
                        var value = new decimal(BitConverter.ToInt32(matchRule.Value),
                            BitConverter.ToInt32(matchRule.Value, sizeof(int)),
                            BitConverter.ToInt32(matchRule.Value, sizeof(int) * 2), matchRule.Value[15] == 0x80,
                            matchRule.Value[14]);
                        AddMatchParameter(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.String:
                    {
                        var value = Encoding.UTF8.GetString(matchRule.Value);
                        AddMatchParameterString(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    case ValueType.DateTime:
                    {
                        var value = DateTime.FromBinary(BitConverter.ToInt64(matchRule.Value));
                        AddMatchParameterDateTime(property, matchRule.FilterMatchMode, sqlParameterCounterAsString, value);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            //TODO: User defined logical operation + scopes
            rawSql += string.Join(" AND ", rawSqlMatchParameters.Select(_ => _.Key + '@' + _.Value.name));
            
            //Sort

            var rawSqlSortParameters = new List<string>();
            if (filterSortModel.SortRules.Any())
                rawSql += " ORDER BY ";
        
            foreach (var sortRule in filterSortModel.SortRules)
            {
                var property = entityType.GetProperty(sortRule.Key);
            
                if (property == null || property.GetCustomAttribute<AllowFilterSortAttribute>(true) == null)
                    throw new CustomException(Localize.Error.FilterSortModelPropertyUnavailable);

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
            
            // ReSharper disable once CoVariantArrayConversion
            var query = FromSql(rawSql, rawSqlMatchParameters.Select(_ => _.Value.parameter).ToArray());

            return (query.Count(), query.GetPage(pageModel));
        }
    }
}