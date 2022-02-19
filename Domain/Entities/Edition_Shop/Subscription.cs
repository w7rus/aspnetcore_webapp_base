using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class Subscription : EntityBase<Guid>
{
    /// <summary>
    /// Alias of the Subscription
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of the Subscription
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Price of the Subscription
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Currency of a Subscription
    /// </summary>
    [StringLength(3)]
    public string Currency { get; set; }

    /// <summary>
    /// Metadata of the Subscription
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a Company this Subscription references
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company this Subscription references
    /// </summary>
    public virtual Company Company { get; set; }

    /// <summary>
    /// [Proxy]
    /// Stores referencing this Subscription
    /// </summary>
    public virtual ICollection<Store> Stores { get; set; }
}