using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInviteUserDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}