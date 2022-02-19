using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class ProductGroup : EntityBase<Guid>
{
    /// <summary>
    /// Alias of a ProductGroup
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of a ProductGroup
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Metadata of a ProductGroup
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a TaxCategory this ProductGroup references
    /// </summary>
    public Guid? TaxCategoryId { get; set; }

    /// <summary>
    /// TaxCategory this ProductGroup references
    /// </summary>
    public virtual TaxCategory TaxCategory { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroup mappings to Categories
    /// </summary>
    public virtual ICollection<ProductGroupToCategoryMapping> ProductGroupToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroup mappings to Products
    /// </summary>
    public virtual ICollection<ProductGroupToProductMapping> ProductGroupToProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroups referencing this ProductGroup
    /// </summary>
    public virtual ICollection<CompanyProductGroup> CompanyProductGroups { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroup mappings to Products
    /// A collection of other Products that are referencing this ProductGroup as its Products can be added in addition to referencing Product in Cart
    /// </summary>
    public virtual ICollection<RelatedProductGroupToProductMapping> RelatedProductGroupToProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroup mappings to ProductGroups
    /// A collection of other ProductGroups Products that can be added in addition to current ProductGroup Products in Cart
    /// </summary>
    public virtual ICollection<RelatedProductGroupToProductGroupMapping> RelatedProductGroupToProductGroupMappings
    {
        get;
        set;
    }

    /// <summary>
    /// [Proxy]
    /// ProductGroup mappings to ProductGroups
    /// A collection of other ProductGroups that are referencing this ProductGroup as its Products can be added in addition to referencing ProductGroup Products in Cart
    /// </summary>
    public virtual ICollection<RelatedProductGroupToProductGroupMapping>
        RelatedProductGroupToProductGroupMappingsReverse { get; set; }
}