using System.Security.Cryptography;
using System.Text;

namespace ShieldText.Services;

public class CryptoService
{
    // تعداد بایت‌های Salt و IV
    private const int SaltSize = 16;
    private const int IvSize = 12;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string Encrypt(string plainText, string password)
    {
        // ۱. Salt تصادفی بساز
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // ۲. IV تصادفی بساز
        byte[] iv = RandomNumberGenerator.GetBytes(IvSize);

        // ۳. از password یه Key بساز
        byte[] key = DeriveKey(password, salt);

        // ۴. متن رو به بایت تبدیل کن
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        // ۵. Encrypt کن با AES-GCM
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(key, 16);
        aes.Encrypt(iv, plainBytes, cipherBytes, tag);

        // ۶. همه چیز رو کنار هم بذار: salt + iv + tag + cipher
        byte[] result = new byte[SaltSize + IvSize + 16 + cipherBytes.Length];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(iv, 0, result, SaltSize, IvSize);
        Buffer.BlockCopy(tag, 0, result, SaltSize + IvSize, 16);
        Buffer.BlockCopy(cipherBytes, 0, result, SaltSize + IvSize + 16, cipherBytes.Length);

        // ۷. خروجی رو Base64 کن
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText, string password)
    {
        // ۱. Base64 رو به بایت برگردون
        byte[] allBytes = Convert.FromBase64String(cipherText);

        // ۲. بخش‌های مختلف رو جدا کن
        byte[] salt = new byte[SaltSize];
        byte[] iv = new byte[IvSize];
        byte[] tag = new byte[16];
        byte[] cipherBytes = new byte[allBytes.Length - SaltSize - IvSize - 16];

        Buffer.BlockCopy(allBytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(allBytes, SaltSize, iv, 0, IvSize);
        Buffer.BlockCopy(allBytes, SaltSize + IvSize, tag, 0, 16);
        Buffer.BlockCopy(allBytes, SaltSize + IvSize + 16, cipherBytes, 0, cipherBytes.Length);

        // ۳. Key رو از password بساز
        byte[] key = DeriveKey(password, salt);

        // ۴. Decrypt کن
        byte[] plainBytes = new byte[cipherBytes.Length];

        using var aes = new AesGcm(key, 16);
        aes.Decrypt(iv, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private byte[] DeriveKey(string password, byte[] salt)
    {
        // PBKDF2 - از password یه key قوی میسازه
        return Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize
        );
    }
}