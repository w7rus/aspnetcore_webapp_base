using System.ComponentModel.DataAnnotations;
using Common.Models;

namespace DTO.Models.Application;

public class ApplicationSetup
{
    [Required]
    [RegularExpression(RegexExpressions.Email)]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}