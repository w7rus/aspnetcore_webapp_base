using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.UserGroup;

public class UserGroupCreateDto
{
    [Required]
    [MaxLength(32)]
    public string Alias { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }
    
    public long Priority { get; set; }
    public Guid TargetUserId { get; set; }
}