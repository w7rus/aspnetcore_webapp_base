using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Data;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Base
{
    public interface IRepositoryBase<TEntity, in TKey> where TEntity : EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        void Add(TEntity entity);
        void Add(ICollection<TEntity> entities);
        Task AddAsync(ICollection<TEntity> entities);
        void Save(TEntity entity);
        void Save(ICollection<TEntity> entities);
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
            entity.CreatedAt = DateTimeOffset.UtcNow;

            DbSet.Add(entity);
        }

        public void Add(ICollection<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            DbSet.AddRange(entities);
        }

        public async Task AddAsync(ICollection<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }

            await DbSet.AddRangeAsync(entities);
        }

        public void Save(TEntity entity)
        {
            if (entity.IsNew())
                entity.CreatedAt = DateTimeOffset.UtcNow;
            else
            {
                var entityEntry = AppDbContext.Entry(entity);
                entity.UpdatedAt = entityEntry.State switch
                {
                    EntityState.Modified when !entityEntry.Property(nameof(EntityBase<TKey>.UpdatedAt)).IsModified =>
                        DateTimeOffset.UtcNow,
                    EntityState.Detached => throw new InvalidOperationException("Can't save detached entity."),
                    _ => entity.UpdatedAt
                };
            }

            DbSet.Update(entity);
        }

        public void Save(ICollection<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.IsNew())
                    entity.CreatedAt = DateTimeOffset.UtcNow;
                else
                {
                    var entityEntry = AppDbContext.Entry(entity);
                    entity.UpdatedAt = entityEntry.State switch
                    {
                        EntityState.Modified when !entityEntry.Property(nameof(EntityBase<TKey>.UpdatedAt)).IsModified
                            => DateTimeOffset.UtcNow,
                        EntityState.Detached => throw new InvalidOperationException("Can't save detached entity."),
                        _ => entity.UpdatedAt
                    };
                }
            }

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
    }
}