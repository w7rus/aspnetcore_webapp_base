using System.Collections.Generic;

namespace Common.Models;

public interface IWarningModelResult
{
    public List<KeyValuePair<string, string>> Warnings { get; set; }
}

public class WarningModelResult : IWarningModelResult
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<KeyValuePair<string, string>> Warnings { get; set; }
}