using System;
using System.ComponentModel.DataAnnotations;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupUpdateDto
{
    public Guid UserGroupId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Alias { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }
    
    [Range(long.MinValue, long.MaxValue)]
    public long Priority { get; set; }
}