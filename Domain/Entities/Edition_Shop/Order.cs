using System;
using System.Collections.Generic;
using Domain.Entities.Base;
using Domain.Enums.Edition_Shop;

namespace Domain.Entities.Edition_Shop;

public class Order : EntityBase<Guid>
{
    /// <summary>
    /// ShippingAddress of an Order
    /// </summary>
    public string ShippingAddress { get; set; }

    /// <summary>
    /// Note of an Order
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// OrderStatus of an Order
    /// </summary>
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// PaymentStatus of an Order
    /// </summary>
    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// OrderType of an Order
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// Metadata of an Order
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a User this Order references
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// [Proxy]
    /// User this Order references
    /// </summary>
    public virtual User User { get; set; }

    /// <summary>
    /// OrderItems referencing this Order
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}