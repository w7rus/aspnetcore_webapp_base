using Common.Models;
using DTO.Models.Base;

namespace DTO.Models.Permission;

public class PermissionReadCollectionDto : IEntityCollectionBaseDto
{
    public PageModel PageModel { get; set; }
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}