using System.ComponentModel.DataAnnotations;

namespace CocktailQuest.Api.Dtos.Auth;

/// <summary>
/// Credenciales para iniciar sesión.
/// </summary>
public class LoginRequest
{
    /// <example>matias@example.com</example>
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    /// <example>Negroni2026!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}
