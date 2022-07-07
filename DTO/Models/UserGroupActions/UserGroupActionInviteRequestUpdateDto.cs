using System;
using Common.Enums;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionInviteRequestUpdateDto
{
    public Guid UserGroupInviteRequestId { get; set; }
    public Choice Choice { get; set; }
}