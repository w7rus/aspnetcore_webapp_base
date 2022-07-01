using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.Models.UserGroup;

public class UserGroupUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Alias { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }
    
    public long Priority { get; set; }
}