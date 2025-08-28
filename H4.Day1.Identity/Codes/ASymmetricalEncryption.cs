using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace H4.Day1.Identity.Codes;

public class ASymmetricalEncryption
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string _publicKey;
    private string _privateKey;
    private const string KeyFolder = "Keys";
    private const string PublicKeyFile = "Keys/public.key";
    private const string PrivateKeyFile = "Keys/private.key";

    public ASymmetricalEncryption(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        if (!Directory.Exists(KeyFolder))
            Directory.CreateDirectory(KeyFolder);

        if (File.Exists(PublicKeyFile) && File.Exists(PrivateKeyFile))
        {
            _publicKey = File.ReadAllText(PublicKeyFile);
            _privateKey = File.ReadAllText(PrivateKeyFile);
        }
        else
        {
            using (RSA rsa = RSA.Create(2048))
            {
                byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
                _privateKey = "-----BEGIN PRIVATE KEY-----\n" +
                              Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                              "\n-----END PRIVATE KEY-----";

                byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                _publicKey = "-----BEGIN PUBLIC KEY-----\n" +
                             Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                             "\n-----END PUBLIC KEY-----";

                File.WriteAllText(PublicKeyFile, _publicKey);
                File.WriteAllText(PrivateKeyFile, _privateKey);
            }
        }
    }

    public string DecryptASymmetrical(string textToDecrypt)
    {
        if (string.IsNullOrWhiteSpace(textToDecrypt))
            return string.Empty;

        try
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            string privateKey = _privateKey
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", "").Replace("\r", "").Trim();

            byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            byte[] byteArrayTextToDecrypt = Convert.FromBase64String(textToDecrypt);
            byte[] decryptedDataAsByteArray = rsa.Decrypt(byteArrayTextToDecrypt, true);
            string decryptedDataAsString = Encoding.UTF8.GetString(decryptedDataAsByteArray);

            return decryptedDataAsString;
        }
        catch (CryptographicException)
        {
            return "[Decryption failed]";
        }
        catch (FormatException)
        {
            return "[Invalid data format]";
        }
    }

    // Encrypt should be inside WebApi, you shouldn't place encrypt and decrypt in the same app
    //public string EncryptASymmetrical(string textToEncrypt)
    //{
    //    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
    //    {
    //        string base64Key = _publicKey
    //            .Replace("-----BEGIN PUBLIC KEY-----", "")
    //            .Replace("-----END PUBLIC KEY-----", "")
    //            .Replace("\n", "").Replace("\r", "").Trim();

    //        byte[] publicKeyBytes = Convert.FromBase64String(base64Key);
    //        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

    //        byte[] byteArrayTextToEncrypt = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
    //        byte[] encryptedDataAsByteArray = rsa.Encrypt(byteArrayTextToEncrypt, true);
    //        var encryptedDataAsString = Convert.ToBase64String(encryptedDataAsByteArray);

    //        return encryptedDataAsString;
    //    }
    //}

    public async Task<string> EncryptASymmetrical_WebApi(string dataToEncrypt)
    {
        string? responseMessage = null;

        string[] ar = [_publicKey, dataToEncrypt];
        var arSerialized = JsonSerializer.Serialize(ar);
        var sc = new StringContent(
            arSerialized,
            Encoding.UTF8,
            "application/json"
        );

        using var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsync(
            "https://localhost:7200/encryptor",
            sc);
        responseMessage = await response.Content.ReadAsStringAsync();

        return responseMessage;
    }
}
