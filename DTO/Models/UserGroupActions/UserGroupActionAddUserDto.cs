using System;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionAddUserDto
{
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
}