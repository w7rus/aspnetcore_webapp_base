using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupJoinDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}