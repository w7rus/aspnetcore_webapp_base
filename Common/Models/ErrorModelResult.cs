using System.Collections.Generic;

namespace Common.Models;

public interface IErrorModelResult
{
    public List<KeyValuePair<string, string>> Errors { get; set; }
}

public class ErrorModelResult : IErrorModelResult
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<KeyValuePair<string, string>> Errors { get; set; }
}