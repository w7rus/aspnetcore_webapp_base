using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdateDto
{
    public Guid Id { get; set; }
    [Required]
    public byte[] Value { get; set; }
}