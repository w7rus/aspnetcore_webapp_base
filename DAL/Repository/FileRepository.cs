using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IFileRepository<TEntity> : IRepositoryBase<TEntity, Guid> where TEntity : File
{
}

public class FileRepository<TEntity> : RepositoryBase<TEntity, Guid>, IFileRepository<TEntity> where TEntity : File
{
    public FileRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}