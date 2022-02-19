using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class TaxCategory : EntityBase<Guid>
{
    /// <summary>
    /// Alias of a TaxCategory
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of a TaxCategory
    /// </summary>
    public string Description { get; set; }


    /// <summary>
    /// Taxcode of a TaxCategory
    /// </summary>
    /// TODO: EF HasDefaultValue txcd_00000000
    [StringLength(13)]
    public string Taxcode { get; set; }

    /// <summary>
    /// [Proxy]
    /// Products referencing this TaxCategory
    /// </summary>
    public virtual ICollection<Product> Products { get; set; }

    /// <summary>
    /// [Proxy]
    /// ProductGroups referencing this TaxCategory
    /// </summary>
    public virtual ICollection<ProductGroup> ProductGroups { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProducts referencing this TaxCategory
    /// </summary>
    public virtual ICollection<CompanyProduct> CompanyProducts { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroups referencing this TaxCategory
    /// </summary>
    public virtual ICollection<CompanyProductGroup> CompanyProductGroups { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProducts referencing this TaxCategory
    /// </summary>
    public virtual ICollection<StoreProduct> StoreProducts { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroups referencing this TaxCategory
    /// </summary>
    public virtual ICollection<StoreProductGroup> StoreProductGroups { get; set; }
}