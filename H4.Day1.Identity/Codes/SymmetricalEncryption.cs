using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace H4.Day1.Identity.Codes;

public class SymmetricalEncryption(IDataProtectionProvider key)
{
    private readonly IDataProtector _key = key.CreateProtector(new RSACryptoServiceProvider().ToXmlString(false));

    public string Encrypt(string plainText)
    {
        return _key.Protect(plainText);
    }

    public string Decrypt(string plainText)
    {
        return _key.Unprotect(plainText);
    }
}
