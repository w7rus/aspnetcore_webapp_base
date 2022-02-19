using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class Ad : EntityBase<Guid>
{
    /// <summary>
    /// Alias of the Ad
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Description of the Ad
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Date from Ad should be active
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// Date until Ad should be active 
    /// </summary>
    public DateTimeOffset ValidUntil { get; set; }

    /// <summary>
    /// Metadata of the Ad
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Id of a File this Ad references
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// [Proxy]
    /// File this Ad references
    /// </summary>
    public virtual File File { get; set; }

    /// <summary>
    /// Id of a Company this Ad references
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Company this Ad references
    /// </summary>
    public virtual Company Company { get; set; }
}