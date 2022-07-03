using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.Generic;

public class OkResultDto : IDtoResultBase
{
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}