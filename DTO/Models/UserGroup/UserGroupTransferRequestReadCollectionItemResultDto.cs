using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupTransferRequestReadCollectionItemResultDto : IEntityBaseResultDto<Guid>
{
    public Guid UserGroupId { get; set; }
    public Guid SrcUserId { get; set; }
    public Guid DestUserId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}