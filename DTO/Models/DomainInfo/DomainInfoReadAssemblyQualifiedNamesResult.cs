using System.Collections.Generic;
using Common.Models.Base;

namespace DTO.Models.DomainInfo;

public class DomainInfoReadAssemblyQualifiedNamesResult : DTOResultBase
{
    public IEnumerable<string> AssemblyQualifiedNames { get; set; }
}