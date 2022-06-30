using System;
using Common.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupUpdateResultDto : DTOResultBase
{
    public Guid Id { get; set; }
    public string Alias { get; set; }
    public string Description { get; set; }
    public long Priority { get; set; }
    public bool IsSystem { get; set; }
    public Guid UserId { get; set; }
}