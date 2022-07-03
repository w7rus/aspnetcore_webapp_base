using System;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}