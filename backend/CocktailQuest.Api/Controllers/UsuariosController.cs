using System.Security.Claims;
using CocktailQuest.Api.Data;
using CocktailQuest.Api.Dtos.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CocktailQuest.Api.Controllers;

/// <summary>
/// Datos del usuario autenticado. Todos los endpoints requieren token.
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Authorize]
[Produces("application/json")]
public class UsuariosController : ControllerBase
{
    private readonly CocktailQuestContext _context;

    public UsuariosController(CocktailQuestContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Devuelve el perfil del usuario dueño del token.
    /// </summary>
    /// <response code="200">Perfil del usuario.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    /// <response code="404">El usuario del token ya no existe.</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObtenerPerfil()
    {
        // El claim "sub" (subject) lo puso TokenService con el Id del usuario.
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(sub, out var usuarioId))
        {
            return Unauthorized();
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "El usuario no existe.");
        }

        return UsuarioResponse.Desde(usuario);
    }
}
