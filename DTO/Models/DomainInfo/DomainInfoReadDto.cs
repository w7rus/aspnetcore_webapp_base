using System.ComponentModel.DataAnnotations;

namespace DTO.Models.DomainInfo;

public class DomainInfoReadDto
{
    [Required]
    public string AssemblyQualifiedName { get; set; }
}