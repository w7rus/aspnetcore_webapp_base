using System;
using Domain.Enums;

namespace DTO.Models.File;

public class FileUpdate
{
    public Guid Id { get; set; }
    public AgeRating AgeRating { get; set; }
}