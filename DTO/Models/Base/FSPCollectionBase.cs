using System.ComponentModel.DataAnnotations;
using Common.Models;

namespace DTO.Models.Base;

public class FSPCollectionBase
{
    [Required]
    public PageModel PageModel { get; set; }
    [Required]
    public FilterExpressionModel FilterExpressionModel { get; set; }
    [Required]
    public FilterSortModel FilterSortModel { get; set; }
}