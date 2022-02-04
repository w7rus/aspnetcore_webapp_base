using System;
using Common.Models.Base;

namespace DTO.Models.File;

public class FileCreateResult : DTOResultBase
{
    public Guid Id { get; set; }
}