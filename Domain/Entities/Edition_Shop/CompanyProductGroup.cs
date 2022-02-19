using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class CompanyProductGroup : EntityBase<Guid>
{
    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup mappings to Categories
    /// </summary>
    public virtual ICollection<CompanyProductGroupToCategoryMapping> CompanyProductGroupToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup mappings to Products
    /// </summary>
    public virtual ICollection<CompanyProductGroupToCompanyProductMapping> CompanyProductGroupToCompanyProductMappings
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
    /// Id of a ProductGroup this CompanyProductGroup references
    /// </summary>
    public Guid ProductGroupId { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroup this CompanyProductGroup references
    /// </summary>
    public virtual ProductGroup ProductGroup { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroups referencing this CompanyProductGroup
    /// </summary>
    public virtual ICollection<StoreProductGroup> StoreProductGroups { get; set; }

    /// <summary>
    /// Id of a Company this CompanyProductGroup references
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company this CompanyProductGroup references
    /// </summary>
    public virtual Company Company { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup mappings to CompanyProducts
    /// A collection of other CompanyProducts that are referencing this CompanyProductGroup as its CompanyProducts can be added in addition to referencing CompanyProduct in Cart
    /// </summary>
    public virtual ICollection<RelatedCompanyProductGroupToCompanyProductMapping>
        RelatedCompanyProductGroupToCompanyProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup mappings to CompanyProductGroups
    /// A collection of other CompanyProductGroups CompanyProducts that can be added in addition to current CompanyProductGroup CompanyProducts in Cart
    /// </summary>
    public virtual ICollection<RelatedCompanyProductGroupToCompanyProductGroupMapping>
        RelatedCompanyProductGroupToCompanyProductGroupMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroup mappings to CompanyProductGroups
    /// A collection of other CompanyProductGroups that are referencing this CompanyProductGroup as its CompanyProducts can be added in addition to referencing CompanyProductGroup CompanyProducts in Cart
    /// </summary>
    public virtual ICollection<RelatedCompanyProductGroupToCompanyProductGroupMapping>
        RelatedCompanyProductGroupToCompanyProductGroupMappingsReverse { get; set; }
}