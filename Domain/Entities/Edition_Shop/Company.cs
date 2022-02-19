using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class Company : EntityBase<Guid>
{
    /// <summary>
    /// Name of the Company
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Address of the Company
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Is Company Active?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Metadata of the Company
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a File this Company references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this Company references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company mappings to Users
    /// </summary>
    public virtual ICollection<UserToCompanyMapping> UserToCompanyMappings { get; set; }

    /// <summary>
    /// [Proxy]
    /// Discounts referencing this Company
    /// </summary>
    public virtual ICollection<Discount> Discounts { get; set; }

    /// <summary>
    /// [Proxy]
    /// Subscriptions referencing this Company
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; }

    /// <summary>
    /// [Proxy]
    /// Ads referencing this Company
    /// </summary>
    public virtual ICollection<Ad> Ads { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductGroups referencing this Company
    /// </summary>
    public virtual ICollection<CompanyProductGroup> CompanyProductGroups { get; set; }

    /// <summary>
    /// [Proxy]
    /// CompanyProductOverrides referencing this Company
    /// </summary>
    public virtual ICollection<CompanyProduct> CompanyProducts { get; set; }
}