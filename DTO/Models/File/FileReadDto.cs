using System;
using DTO.Models.Base;

namespace DTO.Models.File;

public class FileReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}