using System.Text.RegularExpressions;

namespace AgroSolutions.Identity.Application.Validators;

public static class RegisterValidator
{
    public static void Validate(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O Nome é obrigatório");

        if (name.Length < 3)
            throw new ArgumentException("O Nome precisa de no mínimo 3 caracteres");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O Email é obrigatório");

        if (!IsValidEmail(email))
            throw new ArgumentException("Formato de email inválido");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("A Senha é obrigatória");

        if (password.Length < 6)
            throw new ArgumentException("A Senha precia de no mínimo 6 caracteres");

        if (!HasUpperCase(password))
            throw new ArgumentException("A Senha precisa ter no mínimo uma letra maiúscula");

        if (!HasLowerCase(password))
            throw new ArgumentException("A Senha precisa ter no mínimo uma letra minúscula");

        if (!HasDigit(password))
            throw new ArgumentException("A Senha precia de no mínimo 1 dígito");
    }

    private static bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private static bool HasUpperCase(string password) => password.Any(char.IsUpper);
    private static bool HasLowerCase(string password) => password.Any(char.IsLower);
    private static bool HasDigit(string password) => password.Any(char.IsDigit);
}