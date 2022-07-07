using System;
using Common.Enums;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupTransferRequestUpdateDto
{
    public Guid UserGroupTransferRequestId { get; set; }
    public Choice Choice { get; set; }
}