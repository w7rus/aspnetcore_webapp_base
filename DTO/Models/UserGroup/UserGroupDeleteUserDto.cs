using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupDeleteUserDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}