using Common.Models;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInviteRequestReadCollectionDto : IEntityCollectionBaseDto
{
    public PageModel PageModel { get; set; }
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}