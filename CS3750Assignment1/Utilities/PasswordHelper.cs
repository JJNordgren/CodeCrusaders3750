using System.Security.Cryptography;
using System.Text;

namespace CS3750Assignment1.Utilities
{
    public static class PasswordHelper
    {
        // Generate a hashed password with a random salt
        public static string HashPassword(string plainPassword, out string salt)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // Combine password and salt
            string saltedPassword = plainPassword + salt;

            // Hash the salted password
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Verify if the plain password matches the stored hash
        public static bool VerifyPassword(string plainPassword, string storedHash, string salt)
        {
            // Combine plain password and salt
            string saltedPassword = plainPassword + salt;

            // Hash the salted password
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                string hashToCompare = Convert.ToBase64String(hashBytes);

                // Compare the computed hash with the stored hash
                return hashToCompare == storedHash;
            }
        }
    }
}
