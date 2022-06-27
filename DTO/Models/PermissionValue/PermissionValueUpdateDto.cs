using System;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdateDto
{
    public Guid Id { get; set; }
    public byte[] Value { get; set; }
}