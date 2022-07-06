using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupInviteRequestReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}