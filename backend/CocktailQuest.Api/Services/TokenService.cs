using System.Text;
using CocktailQuest.Api.Configuration;
using CocktailQuest.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CocktailQuest.Api.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly JsonWebTokenHandler _handler = new();

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public TokenGenerado GenerarToken(Usuario usuario)
    {
        var expiraEn = DateTime.UtcNow.AddMinutes(_settings.ExpiracionMinutos);
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiraEn,
            SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256),
            // Los "claims" son los datos que viajan dentro del token.
            // No van datos sensibles: el contenido de un JWT se puede leer (está firmado, no cifrado).
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
                [JwtRegisteredClaimNames.UniqueName] = usuario.NombreUsuario,
                [JwtRegisteredClaimNames.Email] = usuario.Email,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
            }
        };

        return new TokenGenerado(_handler.CreateToken(descriptor), expiraEn);
    }
}
