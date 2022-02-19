using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Edition_Shop;

public class Product : EntityBase<Guid>
{
    /// <summary>
    /// Alias of a Product
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of a Product
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Price of a Product
    /// To be provided in a Currency’s smallest unit
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Currency of a Product
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; }

    /// <summary>
    /// Metadata of a Product
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of the File this Product references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this Product references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// Id of the TaxCategory this Product references
    /// </summary>
    public Guid? TaxCategoryId { get; set; }

    /// <summary>
    /// [Proxy]
    /// TaxCategory this Product references
    /// </summary>
    public virtual TaxCategory TaxCategory { get; set; }

    /// <summary>
    /// [Proxy]
    /// Product mappings to ProductGroup
    /// </summary>
    public virtual ICollection<ProductGroupToProductMapping> ProductGroupToProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Product mappings to Categories
    /// </summary>
    public virtual ICollection<ProductToCategoryMapping> ProductToCategoryMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Product mappings to CompanyProductGroup
    /// </summary>
    public virtual ICollection<CompanyProductGroupToCompanyProductMapping> CompanyProductGroupToProductMappings
    {
        get;
        set;
    }

    /// <summary>
    /// [Proxy]
    /// Product mappings to StoreProductGroup
    /// </summary>
    public virtual ICollection<StoreProductGroupToStoreProductMapping> StoreProductGroupToProductMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Product mappings to ProductGroup
    /// A collection of other ProductGroups Products that can be added in addition to current Product in Cart
    /// </summary>
    public virtual ICollection<RelatedProductGroupToProductMapping> RelatedProductGroupToProductMappings { get; set; }
}