using System;
using Domain.Enums;
using DTO.Models.Base;

namespace DTO.Models.File;

public class FileUpdateDto
{
    public Guid FileId { get; set; }
    public AgeRating AgeRating { get; set; }
}