using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Common.Models;

namespace DTO.Models.Base;

public interface IEntityCollectionBaseDto
{
    public PageModel PageModel { get; set; }

    public FilterExpressionModel FilterExpressionModel { get; set; }
    
    public FilterSortModel FilterSortModel { get; set; }
}