using System;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueDeleteDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}