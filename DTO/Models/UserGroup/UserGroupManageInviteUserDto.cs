using System;
using Common.Enums;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupManageInviteUserDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    public Choice Choice { get; set; }
}