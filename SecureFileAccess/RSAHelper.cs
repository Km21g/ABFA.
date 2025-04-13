using System;
using System.Security.Cryptography;
using System.Text;

public static class RSAHelper
{
    private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

    public static string Encrypt(string data)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] encryptedBytes = rsa.Encrypt(dataBytes, false);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string encryptedData)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
        byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}



