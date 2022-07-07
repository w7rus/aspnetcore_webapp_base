using Common.Models;
using DTO.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueReadCollectionDto : IEntityCollectionBaseDto
{
    public PageModel PageModel { get; set; }
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}