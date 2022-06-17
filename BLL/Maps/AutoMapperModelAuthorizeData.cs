using System.Collections.Generic;
using Common.Models;
using Domain.Entities;

namespace BLL.Maps;

public class AutoMapperModelAuthorizeData
{
    public Dictionary<string, bool> FieldAuthorizeResultDictionary { get; set; }
}