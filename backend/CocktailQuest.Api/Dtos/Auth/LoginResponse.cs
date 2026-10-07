using CocktailQuest.Api.Dtos.Usuarios;

namespace CocktailQuest.Api.Dtos.Auth;

/// <summary>
/// Respuesta de un login exitoso.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT. Se manda en cada request protegido con el header "Authorization: Bearer {token}".
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora (UTC) en que el token deja de ser válido.
    /// </summary>
    public DateTime ExpiraEn { get; set; }

    /// <summary>
    /// Datos públicos del usuario que inició sesión.
    /// </summary>
    public UsuarioResponse Usuario { get; set; } = new();
}
