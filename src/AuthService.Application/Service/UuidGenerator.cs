using System.Security.Cryptography;
using System.Text;

namespace AuthService.Application.Services;

public static class UuidGenerator
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string GenerateShortUUID()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[12];
        rng.GetBytes(bytes);

        var result = new StringBuilder(12);

        for (int i = 0; i < 12; i++)
        {
            result.Append(Alphabet[bytes[i] % Alphabet.Length]); // Se utiliza el módulo para asegurar que el índice esté dentro del rango del alfabeto
        }

        return result.ToString();
    }

    public static string GenerateUserId()
    {
        return $"usr_{GenerateShortUUID()}";
    }

    public static string GenerateRoleId()
    {
        return $"rol_{GenerateShortUUID()}";
    }

    public static bool IsValidUserId(string userId)
    {
        if(string.IsNullOrEmpty(userId))
        {
            return false;
        }
        if(userId.Length != 12 || !userId.StartsWith("usr_"))
        {
            return false;
        }

        // Se crea una variable donde solo contenga los caracteres del ID sin el prefijo "usr_"
        var idPart = userId[4..];
        return idPart.All(c => Alphabet.Contains(c)); // Se verifica que todos los caracteres del ID estén en el alfabeto permitido
    }
}