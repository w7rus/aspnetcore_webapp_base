using System;
using System.ComponentModel.DataAnnotations;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdateDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    [Required]
    public byte[] Value { get; set; }
}