using System;
using System.ComponentModel.DataAnnotations;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdateDto
{
    public Guid PermissionValueId { get; set; }
    [Required]
    public byte[] Value { get; set; }
}