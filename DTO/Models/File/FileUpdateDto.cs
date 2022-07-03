using System;
using Domain.Enums;
using DTO.Models.Base;

namespace DTO.Models.File;

public class FileUpdateDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    public AgeRating AgeRating { get; set; }
}