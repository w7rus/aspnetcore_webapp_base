using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupJoinUserDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}