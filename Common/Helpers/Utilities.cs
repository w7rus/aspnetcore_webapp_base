using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Common.Exceptions;
using Common.Models;

namespace Common.Helpers;

public static class Utilities
{
    public static byte[] GenerateRandomBytes(int byteCount)
    {
        var csp = RandomNumberGenerator.Create();
        var buffer = new byte[byteCount];
        csp.GetBytes(buffer);
        return buffer;
    }

    public static string GenerateRandomBase64String(int byteCount)
    {
        return Convert.ToBase64String(GenerateRandomBytes(byteCount));
    }

    public static string ToHttpQueryString(NameValueCollection nameValueCollection)
    {
        return string.Join("&",
            nameValueCollection.AllKeys.Select(_ =>
                HttpUtility.UrlEncode(_) + "=" + HttpUtility.UrlEncode(nameValueCollection[_])));
    }

    public static async Task<string> ToHttpQueryString(object obj)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, obj);
        ms.Seek(0, SeekOrigin.Begin);

        var result = string.Join("&",
            (await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(ms) ??
             throw new CustomException(Localize.Error.ObjectDeserializationFailed)).Select(_ =>
                HttpUtility.UrlEncode(_.Key) + "=" + HttpUtility.UrlEncode(_.Value)));

        return result;
    }
}