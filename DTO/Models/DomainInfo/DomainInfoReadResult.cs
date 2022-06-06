using System.Collections.Generic;
using Common.Enums;
using Common.Models.Base;

namespace DTO.Models.DomainInfo;

public class DomainInfoReadResult : DTOResultBase
{
    public IEnumerable<KeyValuePair<string, ValueType>> PropertiesValueTypes { get; set; }
}