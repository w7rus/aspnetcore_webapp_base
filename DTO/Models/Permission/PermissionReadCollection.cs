using Common.Models;
using Domain.Enums;

namespace DTO.Models.Permission;

public class PermissionReadCollection
{
    public PageModel PageModel { get; set; }
    public FilterMatchModel FilterMatchModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}