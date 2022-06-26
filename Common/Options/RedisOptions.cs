using System.Collections.Generic;
using Common.Models;

namespace Common.Options;

public class RedisOptions
{
    public ICollection<UriData> Endpoints { get; set; }
}