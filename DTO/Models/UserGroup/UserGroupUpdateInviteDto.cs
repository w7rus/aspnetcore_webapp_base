using System;
using Common.Enums;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupUpdateInviteDto : IEntityBaseDto
{
    public Guid Id { get; set; }
    public Choice Choice { get; set; }
}