using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupTransferInitDto
{
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
}