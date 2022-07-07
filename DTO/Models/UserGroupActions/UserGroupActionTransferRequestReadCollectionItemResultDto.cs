using System;
using DTO.Models.Base;

namespace DTO.Models.UserGroupActions;

public class UserGroupActionTransferRequestReadCollectionItemResultDto : IEntityBaseResultDto<Guid>
{
    public Guid UserGroupId { get; set; }
    public Guid SrcUserId { get; set; }
    public Guid DestUserId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}