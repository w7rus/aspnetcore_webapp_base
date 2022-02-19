using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class Category : EntityBase<Guid>
{
    /// <summary>
    /// Alias of the Category
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of the Category
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Metadata of the Category
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to Products
    /// </summary>
    public virtual ICollection<ProductToCategoryMapping> ProductToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to CompanyProductOverrides
    /// </summary>
    public virtual ICollection<CompanyProductToCategoryMapping> CompanyProductOverrideToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to StoreProductOverrides
    /// </summary>
    public virtual ICollection<StoreProductToCategoryMapping> StoreProductOverrideToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to ProductGroups
    /// </summary>
    public virtual ICollection<ProductGroupToCategoryMapping> ProductGroupToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to CompanyProductGroups
    /// </summary>
    public virtual ICollection<CompanyProductGroupToCategoryMapping> CompanyProductGroupToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Category mappings to StoreProductGroups
    /// </summary>
    public virtual ICollection<StoreProductGroupToCategoryMapping> StoreProductGroupToCategoryMappings { get; set; }
}