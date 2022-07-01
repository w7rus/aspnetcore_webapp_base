using System.ComponentModel.DataAnnotations;

namespace DTO.Models.File;

public class FileCDNDeleteDto
{
    [Required]
    public string FileName { get; set; }
}