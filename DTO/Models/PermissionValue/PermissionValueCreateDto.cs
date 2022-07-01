using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.PermissionValue;

public class PermissionValueCreateDto
{
    [Required]
    public byte[] Value { get; set; }
    public Guid PermissionId { get; set; }
    public Guid EntityId { get; set; }
}