using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class Hash
{
    public static string generate(string input, int length = 8)
    {
        string normalized = input.Normalize(NormalizationForm.FormC);
        byte[] inputBytes = Encoding.UTF8.GetBytes(normalized);
        byte[] hashBytes;
        using (SHA256 sha = SHA256.Create()) hashBytes = sha.ComputeHash(inputBytes);
        string base64 = Convert.ToBase64String(hashBytes);
        string urlSafe = base64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        return urlSafe.Substring(0, Math.Min(length, urlSafe.Length));
    }
}