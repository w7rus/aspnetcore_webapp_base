using System;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionTransferRequestCreateDto
{
    public Guid UserGroupId { get; set; }
    public Guid TargetUserId { get; set; }
}