using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupDeleteDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}