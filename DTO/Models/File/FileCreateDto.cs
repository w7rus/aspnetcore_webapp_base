using System;
using Domain.Enums;

namespace DTO.Models.File;

public class FileCreateDto
{
    public AgeRating AgeRating { get; set; }
    public Guid UserId { get; set; }
}