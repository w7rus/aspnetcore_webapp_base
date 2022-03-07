using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Domain.Entities.Edition_Shop;

namespace BLL.Services.Edition_Shop;

public interface ICartItemService : IEntityServiceBase<CartItem>
{
}

public class CartItemService : ICartItemService
{
    public async Task Save(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CartItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CartItem> Create(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}