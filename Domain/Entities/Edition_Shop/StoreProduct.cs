using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class StoreProduct : EntityBase<Guid>
{
    /// <summary>
    /// Alias of the StoreProduct
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of the StoreProduct
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Price of the StoreProduct
    /// To be provided in a Currency’s smallest unit
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// Currency of a StoreProduct
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; }

    /// <summary>
    /// Metadata of the StoreProduct
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of the File this StoreProduct references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this StoreProduct references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// Id of the TaxCategory this StoreProduct references
    /// </summary>
    public Guid? TaxCategoryId { get; set; }

    /// <summary>
    /// [Proxy]
    /// TaxCategory this StoreProduct references
    /// </summary>
    public virtual TaxCategory TaxCategory { get; set; }

    /// <summary>
    /// Id of the CompanyProduct this StoreProduct references.
    /// </summary>
    public Guid CompanyProductId { get; set; }

    /// <summary>
    /// CompanyProduct this StoreProduct references
    /// </summary>
    public virtual CompanyProduct CompanyProduct { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProduct mappings to Discounts
    /// </summary>
    public virtual ICollection<StoreProductToDiscountMapping> StoreProductToDiscountMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProduct mappings to Categories
    /// </summary>
    public virtual ICollection<StoreProductToCategoryMapping> StoreProductToCategoryMappings { get; set; }

    /// <summary>
    /// Id of a Store this StoreProduct references
    /// </summary>
    public Guid StoreId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Store this StoreProduct references
    /// </summary>
    public virtual Store Store { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProduct mappings to StoreProductGroup
    /// A collection of other StoreProductGroups StoreProducts that can be added in addition to current StoreProduct in Cart
    /// </summary>
    public virtual ICollection<RelatedStoreProductGroupToStoreProductMapping>
        RelatedStoreProductGroupToStoreProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// CartItems referencing this StoreProduct
    /// </summary>
    public virtual ICollection<CartItem> CartItems { get; set; }
}