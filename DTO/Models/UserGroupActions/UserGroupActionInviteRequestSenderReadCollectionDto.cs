using System;
using Common.Models;
using DTO.Models.Base;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionInviteRequestSenderReadCollectionDto : IEntityCollectionBaseDto
{
    public Guid UserGroupId { get; set; }
    public PageModel PageModel { get; set; }
    public FilterExpressionModel FilterExpressionModel { get; set; }
    public FilterSortModel FilterSortModel { get; set; }
}