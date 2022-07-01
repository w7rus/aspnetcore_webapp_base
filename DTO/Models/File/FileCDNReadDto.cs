using System.ComponentModel.DataAnnotations;

namespace DTO.Models.File;

public class FileCDNReadDto
{
    [Required]
    public string FileName { get; set; }
}