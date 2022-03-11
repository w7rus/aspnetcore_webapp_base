using System;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class CartItem : EntityBase<Guid>
{
    /// <summary>
    /// Id of an StoreProduct this OrderItem references
    /// </summary>
    public Guid StoreProductId { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProduct this OrderItem references
    /// </summary>
    public virtual StoreProduct StoreProduct { get; set; }

    /// <summary>
    /// Id of a User this CartItem references
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// [Proxy]
    /// User this CartItem references
    /// </summary>
    public virtual User User { get; set; }
    
    /// <summary>
    /// Amount of a CartItem
    /// </summary>
    public int Amount { get; set; }
    
    //TODO: Add CartItem linkage (as if products is a part of of another product)
}