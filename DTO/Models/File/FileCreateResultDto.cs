using System;
using Common.Models.Base;
using Domain.Enums;

namespace DTO.Models.File;

public class FileCreateResultDto : DTOResultBase
{
    public Guid Id { get; set; }
    public AgeRating AgeRating { get; set; }
}