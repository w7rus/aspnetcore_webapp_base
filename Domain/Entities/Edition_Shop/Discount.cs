using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;
using Domain.Enums.Edition_Shop;

namespace Domain.Entities.Edition_Shop;

public class Discount : EntityBase<Guid>
{
    /// <summary>
    /// Alias of a Discount
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Code of a Discount
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Type of a Discount
    /// </summary>
    public DiscountType Type { get; set; }

    /// <summary>
    /// Value of a Discount.
    /// To be provided in a Currency’s smallest unit
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Percentage of a Discount.
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// Currency of a Discount
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; }

    /// <summary>
    /// Date from Discount should be active
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// Date until Discount should be active
    /// </summary>
    public DateTimeOffset ValidUntil { get; set; }

    /// <summary>
    /// Metadata of the Discount
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a Company this Subscription references
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company this Subscription references
    /// </summary>
    public virtual Company Company { get; set; }

    /// <summary>
    /// [Proxy]
    /// Discount mappings to CompanyProductOverrides
    /// </summary>
    public virtual ICollection<CompanyProductToDiscountMapping> CompanyProductOverrideToDiscountMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Discount mappings to StoreProductOverrides
    /// </summary>
    public virtual ICollection<StoreProductToDiscountMapping> StoreProductOverrideToDiscountMappings { get; set; }
}