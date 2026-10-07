using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Services;

/// <summary>
/// Token generado y su fecha de expiración.
/// </summary>
public record TokenGenerado(string Token, DateTime ExpiraEn);

/// <summary>
/// Genera tokens JWT para los usuarios autenticados.
/// </summary>
public interface ITokenService
{
    TokenGenerado GenerarToken(Usuario usuario);
}
