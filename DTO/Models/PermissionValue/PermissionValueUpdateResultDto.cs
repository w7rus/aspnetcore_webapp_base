using System;
using Common.Models.Base;

namespace DTO.Models.PermissionValue;

public class PermissionValueUpdateResultDto : DTOResultBase
{
    public Guid Id { get; set; }
    public byte[] Value { get; set; }
}