using System;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionInviteRequestCreateDto
{
    public Guid UserGroupId { get; set; }
    public Guid TargetUserId { get; set; }
}