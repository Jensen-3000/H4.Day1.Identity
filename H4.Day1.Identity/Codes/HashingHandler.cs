using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace H4.Day1.Identity.Codes;

public class HashingHandler
{
    public dynamic MD5Hashing(dynamic valueToHash) =>
        valueToHash is byte[]? MD5.Create().ComputeHash(valueToHash)
            : Convert.ToBase64String(MD5.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(valueToHash.ToString())));

    public dynamic Sha256Hashing(dynamic valueToHash) =>
        valueToHash is byte[]? SHA256.Create().ComputeHash(valueToHash)
            : Convert.ToBase64String(SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(valueToHash.ToString())));

    public dynamic HMACHashing(dynamic valueToHash) =>
        valueToHash is byte[]? new HMACSHA256(Encoding.ASCII.GetBytes("NielsErMinFavoritLærer"))
                .ComputeHash(valueToHash)
            : Convert.ToBase64String(new HMACSHA256(Encoding.ASCII.GetBytes("NielsErMinFavoritLærer"))
                .ComputeHash(valueToHash.ToString()));

    // When the hashing is getting this advanced,
    // it is better to return byte[] instead of using dynamic
    public dynamic PBKDF2Hashing(dynamic valueToHash) =>
        Rfc2898DeriveBytes.Pbkdf2(
        valueToHash is byte[]? valueToHash
    : Encoding.UTF8.GetBytes(valueToHash),
    Encoding.UTF8.GetBytes("ImSalty"), 10, HashAlgorithmName.SHA256, 32);

    // Uses byte[] as input and output
    public byte[] PBKDF2Hashing(byte[] valueToHash) =>
        Rfc2898DeriveBytes.Pbkdf2(
            valueToHash,
            "ImSalty"u8.ToArray(),
            10,
            HashAlgorithmName.SHA256,
            32
            );

    public string BCryptHashing(string textToHash) =>
        BCrypt.Net.BCrypt.HashPassword(textToHash);

    public bool VerifyBCryptHash(string textToCheck, string hashedText) =>
        BCrypt.Net.BCrypt.Verify(textToCheck, hashedText);

    // BCrypt enhanced entropy
    public string BCryptHashing2(string textToHash) =>
        BCrypt.Net.BCrypt.HashPassword(
            textToHash,
            BCrypt.Net.BCrypt.GenerateSalt(10),
            true,
            HashType.SHA256);

    public bool BCryptVerifyHashing2(string textToCheck, string hashedText) =>
        BCrypt.Net.BCrypt.Verify(
            textToCheck,
            hashedText,
            true,
            HashType.SHA256);
}
