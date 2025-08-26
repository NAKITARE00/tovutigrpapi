using System.Security.Cryptography;

public static class PasswordHelper
{
    public static (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var salt = Convert.ToBase64String(saltBytes);
        var hash = Convert.ToBase64String(
            new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256)
                .GetBytes(32)
        );

        return (hash, salt);
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var enteredHash = Convert.ToBase64String(
            new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256)
                .GetBytes(32)
        );

        return enteredHash == storedHash;
    }
}
