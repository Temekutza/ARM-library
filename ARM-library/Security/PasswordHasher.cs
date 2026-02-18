using System.Security.Cryptography;
using System.Text;

namespace ARM_library.Security
{
    public static class PasswordHasher
    {
        // Совместимо с MySQL: SHA2('password', 256) -> hex string
        public static string Sha256Hex(string input)
        {
            input = input ?? string.Empty;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                for (var i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }
    }
}


