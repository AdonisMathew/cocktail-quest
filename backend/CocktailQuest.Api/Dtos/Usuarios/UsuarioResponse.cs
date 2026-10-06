using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Dtos.Usuarios;

/// <summary>
/// Datos públicos de un usuario. Nunca incluye el PasswordHash.
/// </summary>
public class UsuarioResponse
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Xp { get; set; }
    public int Nivel { get; set; }
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Convierte la entidad de base de datos en el DTO que se devuelve al cliente.
    /// </summary>
    public static UsuarioResponse Desde(Usuario usuario) => new()
    {
        Id = usuario.Id,
        NombreUsuario = usuario.NombreUsuario,
        Email = usuario.Email,
        Xp = usuario.Xp,
        Nivel = usuario.Nivel,
        FechaRegistro = usuario.FechaRegistro
    };
}
