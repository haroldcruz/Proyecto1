using System.Security.Cryptography;
using System.Text;
using System;
public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            // Devuelve el hash en base64 (puedes usar Hex si prefieres)
            return Convert.ToBase64String(hashBytes);
        }
    }
}