using SMSVotingSystem.Application.Interfaces;
using Microsoft.Extensions.Options;
using SMSVotingSystem.Application.Common;
using System.Security.Cryptography;

namespace SMSVotingSystem.Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IOptions<SecuritySettings> settings)
        {
            if (settings?.Value == null)
                throw new ArgumentNullException(nameof(settings), "Security settings are not configured");

            if (string.IsNullOrWhiteSpace(settings.Value.EncryptionKey))
                throw new ArgumentException("EncryptionKey is not configured", nameof(settings));

            _key = Convert.FromBase64String(settings.Value.EncryptionKey);

            if (_key.Length != 16 && _key.Length != 24 && _key.Length != 32)
                throw new ArgumentException("Encryption key must be 16, 24, or 32 bytes long");
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV(); // Generate a fresh random IV for this encryption

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();

            // First write the IV to the memory stream
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;

            // Extract the IV from the first 16 bytes
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        

    }

}