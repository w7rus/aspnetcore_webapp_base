using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.File;

public class FileCDNCreateResultDto : IDtoResultBase
{
    public string FileName { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}