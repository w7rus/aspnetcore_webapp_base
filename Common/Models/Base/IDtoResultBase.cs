using System.Collections.Generic;

namespace Common.Models.Base;

public interface IDtoResultBase : IWarningModelResult, IErrorModelResult
{
    public string TraceId { get; set; }
}