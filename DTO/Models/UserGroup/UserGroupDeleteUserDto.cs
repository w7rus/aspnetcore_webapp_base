using System;

namespace DTO.Models.UserGroup;

public class UserGroupDeleteUserDto
{
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
}