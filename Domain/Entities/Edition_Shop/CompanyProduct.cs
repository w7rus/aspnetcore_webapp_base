using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class CompanyProduct : EntityBase<Guid>
{
    /// <summary>
    /// Alias of the CompanyProduct
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of the CompanyProduct
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Price of the CompanyProduct
    /// To be provided in a Currency’s smallest unit
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// Currency of a CompanyProduct
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; }

    /// <summary>
    /// Metadata of the CompanyProduct
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of the File this CompanyProduct references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this CompanyProduct references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// Id of the TaxCategory this CompanyProduct references
    /// </summary>
    public Guid? TaxCategoryId { get; set; }

    /// <summary>
    /// [Proxy]
    /// TaxCategory this CompanyProduct references
    /// </summary>
    public virtual TaxCategory TaxCategory { get; set; }

    /// <summary>
    /// Id of the Product this CompanyProduct references
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Product this CompanyProduct references
    /// </summary>
    public virtual Product Product { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProduct mappings to Discounts
    /// </summary>
    public virtual ICollection<CompanyProductToDiscountMapping> CompanyProductToDiscountMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProducts referencing this CompanyProduct
    /// </summary>
    public virtual ICollection<StoreProduct> StoreProducts { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProduct mappings to Categories
    /// </summary>
    public virtual ICollection<CompanyProductToCategoryMapping> CompanyProductToCategoryMappings { get; set; }

    /// <summary>
    /// Id of a Company this CompanyProduct references
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company this CompanyProduct references
    /// </summary>
    public virtual Company Company { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProduct mappings to CompanyProductGroup
    /// A collection of other CompanyProductGroups CompanyProducts that can be added in addition to current CompanyProduct in Cart
    /// </summary>
    public virtual ICollection<RelatedCompanyProductGroupToCompanyProductMapping>
        RelatedCompanyProductGroupToCompanyProductMappings { get; set; }
}