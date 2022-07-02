using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupReadDtoReadResultDto : EntityBaseResultDto<Guid>
{
    public string Alias { get; set; }
    public string Description { get; set; }
    public long Priority { get; set; }
    public bool IsSystem { get; set; }
    public Guid UserId { get; set; }
}