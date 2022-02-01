using System;
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
    public ulong Size { get; set; }
    
    /// <summary>
    /// Age rating of the File
    /// </summary>
    public AgeRating AgeRating { get; set; }
    
    public Guid? UserId { get; set; }
    public virtual User User { get; set; }
}