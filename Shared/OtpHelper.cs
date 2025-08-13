using System.Security.Cryptography;
using System.Text;

namespace Shared;

public static class OtpHelper
{
    public static string GenerateNumericCode(int digits)
    {
        // Cryptographically strong numeric OTP with leading zeros
        var max = (int)Math.Pow(10, digits);
        var bytes = RandomNumberGenerator.GetBytes(4);
        var val = BitConverter.ToUInt32(bytes, 0) % (uint)max;
        return val.ToString(new string('0', digits));
    }

    public static string Sha256Hex(string input)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}