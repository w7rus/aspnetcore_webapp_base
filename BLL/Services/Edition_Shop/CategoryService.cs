using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Domain.Entities.Edition_Shop;

namespace BLL.Services.Edition_Shop;

public interface ICategoryService : IEntityServiceBase<Category>
{
}

public class CategoryService : ICategoryService
{
    public async Task Save(Category entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Category entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> Create(Category entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}