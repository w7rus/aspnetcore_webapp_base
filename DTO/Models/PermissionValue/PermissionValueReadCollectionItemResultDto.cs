using System;

namespace DTO.Models.PermissionValue;

public class PermissionValueReadCollectionItemResultDto
{
    public Guid Id { get; set; }
    public byte[] Value { get; set; }
    public Guid PermissionId { get; set; }
    public Guid EntityId { get; set; }
}