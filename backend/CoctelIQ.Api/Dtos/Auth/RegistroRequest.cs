using System.ComponentModel.DataAnnotations;

namespace CoctelIQ.Api.Dtos.Auth;

/// <summary>
/// Datos para registrar un usuario nuevo.
/// </summary>
public class RegistroRequest
{
    /// <summary>
    /// Nombre visible en el ranking. Entre 3 y 50 caracteres: letras, números, guion bajo o punto.
    /// </summary>
    /// <example>bartender_tucu</example>
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "El nombre de usuario solo puede tener letras, números, guion bajo y punto.")]
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario. Se guarda en minúsculas.
    /// </summary>
    /// <example>matias@example.com</example>
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña en texto plano (viaja por HTTPS y se guarda hasheada). Mínimo 8 caracteres.
    /// </summary>
    /// <example>Negroni2026!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
