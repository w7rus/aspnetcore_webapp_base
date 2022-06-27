using System;
using Common.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueCreateResultDto : DTOResultBase
{
    public Guid Id { get; set; }
    public byte[] Value { get; set; }
    public Guid PermissionId { get; set; }
    public Guid EntityId { get; set; }
}