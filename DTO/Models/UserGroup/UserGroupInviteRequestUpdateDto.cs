using System;
using Common.Enums;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInviteRequestUpdateDto
{
    public Guid UserGroupInviteRequestId { get; set; }
    public Choice Choice { get; set; }
}