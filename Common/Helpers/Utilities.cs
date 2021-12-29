using System;
using System.Security.Cryptography;

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
}