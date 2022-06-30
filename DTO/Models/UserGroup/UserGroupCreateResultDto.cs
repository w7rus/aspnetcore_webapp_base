using System;
using Common.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupCreateResultDto : DTOResultBase
{
    public Guid Id { get; set; }
}