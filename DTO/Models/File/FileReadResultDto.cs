using System;
using System.Collections.Generic;
using System.IO;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.File;

public class FileReadResultDto : IEntityBaseResultDto<Guid>, IDtoResultBase
{
    public Stream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}