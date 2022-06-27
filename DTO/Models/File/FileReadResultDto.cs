using System.IO;
using System.Net.Http.Headers;
using Common.Models.Base;

namespace DTO.Models.File;

public class FileReadResultDto : DTOResultBase
{
    public Stream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}