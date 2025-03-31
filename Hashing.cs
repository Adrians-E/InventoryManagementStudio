using System;
using System.Linq;
using System.Security.Cryptography;
using System.Data.SQLite; // Correct library for SQLite

public class Hashing
{
    private const int SaltSize = 16; // 16-byte salt
    private const int HashSize = 32; // 32-byte hash
    private const int Iterations = 100000; // PBKDF2 iterations
    private static string connectionString = "Data Source=loginPanel.db;Version=3;";

    public static string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(int id, string password)
    {
        string storedHash = null;

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Password FROM loginPanel WHERE ID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                object result = cmd.ExecuteScalar();
                if (result != null) { 
                    storedHash = result.ToString(); 
                }  else
                {
                    storedHash = null;
                }
                
            }
        }

        if (string.IsNullOrEmpty(storedHash)) return false;

        byte[] hashBytes = Convert.FromBase64String(storedHash);
        byte[] salt = new byte[SaltSize];
        byte[] storedHashBytes = new byte[HashSize];

        Array.Copy(hashBytes, 0, salt, 0, SaltSize);
        Array.Copy(hashBytes, SaltSize, storedHashBytes, 0, HashSize);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] newHash = pbkdf2.GetBytes(HashSize);
            return storedHashBytes.SequenceEqual(newHash);
        }
    }
}