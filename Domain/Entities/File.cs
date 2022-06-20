using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class File : EntityBase<Guid>
{
    /// <summary>
    /// Name of the File. Unique field.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Size of the File
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Age rating of the File
    /// </summary>
    public AgeRating AgeRating { get; set; }

    /// <summary>
    /// Metadata of the File
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }

    [NotMapped]
    public Stream Stream { get; set; }

    [NotMapped]
    public string ContentType { get; set; }

    /// <summary>
    /// Id of a User this File references
    /// </summary>
    public Guid? UserId { get; set; }
    
    /// <summary>
    /// [Proxy]
    /// User this File references
    /// </summary>
    public virtual User User { get; set; }
}