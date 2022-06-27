using System;
using Domain.Enums;

namespace DTO.Models.File;

public class FileUpdateDto
{
    public Guid Id { get; set; }
    public AgeRating AgeRating { get; set; }
}