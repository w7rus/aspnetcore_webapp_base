using Common.Models;

namespace DTO.Models.Permission;

public class PermissionReadFSPCollectionDto
{
    public PageModel PageModel { get; set; }
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}