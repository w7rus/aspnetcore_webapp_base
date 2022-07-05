using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInitInviteUserDto
{
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
}