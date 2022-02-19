using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class OrderItem : EntityBase<Guid>
{
    /// <summary>
    /// OriginalPricePerUnit of an OrderItem
    /// </summary>
    public decimal OriginalPricePerUnit { get; set; }

    /// <summary>
    /// ActualPricePerUnit of an OrderItem
    /// </summary>
    public decimal ActualPricePerUnit { get; set; }

    /// <summary>
    /// TaxPerUnit of an OrderItem
    /// </summary>
    public decimal TaxPerUnit { get; set; }

    /// <summary>
    /// OriginalPrice of an OrderItem
    /// </summary>
    public decimal OriginalPrice { get; set; }

    /// <summary>
    /// ActualPrice of an OrderItem
    /// </summary>
    public decimal ActualPrice { get; set; }

    /// <summary>
    /// Tax of an OrderItem
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Amount of an OrderItem
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Metadata of an OrderItem
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// [CanBeNull]
    /// Id of an CompanyProductOverride this OrderItem references
    /// </summary>
    public Guid CompanyProductOverrideId { get; set; }

    /// <summary>
    /// [CanBeNull]
    /// [Proxy]
    /// CompanyProductOverride this OrderItem references
    /// </summary>
    public virtual CompanyProduct CompanyProduct { get; set; }

    /// <summary>
    /// [CanBeNull]
    /// Id of an StoreProductOverride this OrderItem references
    /// </summary>
    public Guid StoreProductOverrideId { get; set; }

    /// <summary>
    /// [CanBeNull]
    /// [Proxy]
    /// StoreProductOverride this OrderItem references
    /// </summary>
    public virtual StoreProduct StoreProduct { get; set; }

    /// <summary>
    /// Id of an Order this OrderItem references
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Order this OrderItem references
    /// </summary>
    public virtual Order Order { get; set; }
}