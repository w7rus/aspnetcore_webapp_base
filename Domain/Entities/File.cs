using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class File : EntityBase<Guid>
{
    public string Name { get; set; }
    public long Size { get; set; }
    public AgeRating AgeRating { get; set; }
    public Dictionary<string, string> Metadata { get; set; }

    [NotMapped]
    public Stream Stream { get; set; }

    [NotMapped]
    public string ContentType { get; set; }

    public Guid? UserId { get; set; }
    public virtual User User { get; set; }
}