using System;

namespace DTO.Models.PermissionValue;

public class PermissionValueCreateDto
{
    public byte[] Value { get; set; }
    public Guid PermissionId { get; set; }
    public Guid EntityId { get; set; }
}