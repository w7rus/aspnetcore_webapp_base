using System.Collections.Generic;
using Common.Models;

namespace Common.Options;

public class MiscOptions
{
    public bool SecureCookies { get; set; }
    public UriData FileServer { get; set; }
    public ICollection<UriData> CDNServers { get; set; }
}