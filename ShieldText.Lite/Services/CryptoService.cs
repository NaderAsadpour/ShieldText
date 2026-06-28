using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ShieldText.Lite.Services
{
    public class CryptoService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int IvSize = 16;
        private const int Iterations = 100000;

        public string Encrypt(string plainText, string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            byte[] key = DeriveKey(password, salt);
            byte[] iv = new byte[IvSize];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(iv);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes;

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();
                }
            }

            byte[] result = new byte[SaltSize + IvSize + cipherBytes.Length];
            Array.Copy(salt, 0, result, 0, SaltSize);
            Array.Copy(iv, 0, result, SaltSize, IvSize);
            Array.Copy(cipherBytes, 0, result, SaltSize + IvSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText, string password)
        {
            byte[] allBytes = Convert.FromBase64String(cipherText);

            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IvSize];
            byte[] cipherBytes = new byte[allBytes.Length - SaltSize - IvSize];

            Array.Copy(allBytes, 0, salt, 0, SaltSize);
            Array.Copy(allBytes, SaltSize, iv, 0, IvSize);
            Array.Copy(allBytes, SaltSize + IvSize, cipherBytes, 0, cipherBytes.Length);

            byte[] key = DeriveKey(password, salt);

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private byte[] DeriveKey(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                return pbkdf2.GetBytes(KeySize);
        }
    }
}