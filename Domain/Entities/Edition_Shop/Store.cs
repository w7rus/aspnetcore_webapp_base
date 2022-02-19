using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class Store : EntityBase<Guid>
{
    /// <summary>
    /// Name of a Store
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Is Store Active?
    /// </summary>
    public string IsActive { get; set; }

    /// <summary>
    /// Metadata of the Store
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// [Proxy]
    /// WorkingHours referencing this Store
    /// </summary>
    public virtual ICollection<OperatingTime> WorkingHoursCollection { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductGroups referencing this Store
    /// </summary>
    public virtual ICollection<StoreProductGroup> StoreProductGroups { get; set; }

    /// <summary>
    /// [Proxy]
    /// StoreProductOverrides referencing this Store
    /// </summary>
    public virtual ICollection<StoreProduct> StoreProducts { get; set; }

    /// <summary>
    /// Id of a File this Store references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this Store references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// Id of a Subscription this Store references
    /// </summary>
    public Guid? SubscriptionId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Subscription this Store references
    /// </summary>
    public virtual Subscription Subscription { get; set; }
}