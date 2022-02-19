using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class StoreProductGroup : EntityBase<Guid>
{
    /// <summary>
    /// [Proxy]
    /// StoreProductGroup mappings to Categories
    /// </summary>
    public virtual ICollection<StoreProductGroupToCategoryMapping> StoreProductGroupToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroup mappings to Products
    /// </summary>
    public virtual ICollection<StoreProductGroupToStoreProductMapping> StoreProductGroupToStoreProductMappings
    {
        get;
        set;
    }

    /// <summary>
    /// Id of a TaxCategory this CompanyProductGroup references
    /// </summary>
    public Guid? TaxCategoryId { get; set; }

    /// <summary>
    /// [Proxy]
    /// TaxCategory this CompanyProductGroup references
    /// </summary>
    public virtual TaxCategory TaxCategory { get; set; }

    /// <summary>
    /// Id of a CompanyProductGroupId this StoreProductGroup references
    /// </summary>
    public Guid CompanyProductGroupId { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup this StoreProductGroup references
    /// </summary>
    public virtual CompanyProductGroup CompanyProductGroup { get; set; }

    /// <summary>
    /// Id of a Store this StoreProductGroup references
    /// </summary>
    public Guid StoreId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Store this StoreProductGroup references
    /// </summary>
    public virtual Store Store { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroup mappings to StoreProducts
    /// A collection of other StoreProducts that are referencing this StoreProductGroup as its StoreProducts can be added in addition to referencing StoreProduct in Cart
    /// </summary>
    public virtual ICollection<RelatedStoreProductGroupToStoreProductMapping>
        RelatedStoreProductGroupToStoreProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroup mappings to StoreProductGroups
    /// A collection of other StoreProductGroups StoreProducts that can be added in addition to current StoreProductGroup StoreProducts in Cart
    /// </summary>
    public virtual ICollection<RelatedStoreProductGroupToStoreProductGroupMapping>
        RelatedStoreProductGroupToStoreProductGroupMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroup mappings to StoreProductGroups
    /// A collection of other StoreProductGroups that are referencing this StoreProductGroup as its StoreProducts can be added in addition to referencing StoreProductGroup StoreProducts in Cart
    /// </summary>
    public virtual ICollection<RelatedStoreProductGroupToStoreProductGroupMapping>
        RelatedStoreProductGroupToStoreProductGroupMappingsReverse { get; set; }
}