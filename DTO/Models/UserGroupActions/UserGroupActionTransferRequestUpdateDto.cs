using System;
using Common.Enums;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionTransferRequestUpdateDto
{
    public Guid UserGroupTransferRequestId { get; set; }
    public Choice Choice { get; set; }
}