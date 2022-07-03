using System.Collections.Generic;
using Common.Enums;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.DomainInfo;

public class DomainInfoReadResultDto : IDtoResultBase
{
    public IEnumerable<KeyValuePair<string, ValueType>> PropertiesValueTypes { get; set; }
    public List<WarningModelResultEntry> Warnings { get; set; }
    public List<ErrorModelResultEntry> Errors { get; set; }
    public string TraceId { get; set; }
}