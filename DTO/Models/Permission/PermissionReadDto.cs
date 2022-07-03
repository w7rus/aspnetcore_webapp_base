using System;
using DTO.Models.Base;

namespace DTO.Models.Permission;

public class PermissionReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}