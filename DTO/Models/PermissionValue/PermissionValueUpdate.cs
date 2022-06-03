using System;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdate
{
    public Guid Id { get; set; }
    public byte[] Value { get; set; }
}