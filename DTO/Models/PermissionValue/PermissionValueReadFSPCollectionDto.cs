using Common.Models;

namespace DTO.Models.PermissionValue;

public class PermissionValueReadFSPCollectionDto
{
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
    public PageModel PageModel { get; set; }
}