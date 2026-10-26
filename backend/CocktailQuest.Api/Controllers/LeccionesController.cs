using CocktailQuest.Api.Data;
using CocktailQuest.Api.Dtos.Lecciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CocktailQuest.Api.Controllers;

/// <summary>
/// Lecciones: contenido y preguntas del quiz. Todos los endpoints requieren token.
/// </summary>
[ApiController]
[Route("api/lecciones")]
[Authorize]
[Produces("application/json")]
public class LeccionesController : ControllerBase
{
    private readonly CocktailQuestContext _context;

    public LeccionesController(CocktailQuestContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Devuelve una lección con su contenido y sus preguntas, sin indicar cuál es la opción correcta.
    /// </summary>
    /// <param name="id">Id de la lección.</param>
    /// <response code="200">La lección con sus preguntas y opciones.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    /// <response code="404">No existe una lección con ese id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LeccionDetalleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeccionDetalleResponse>> Obtener(int id)
    {
        // Include + ThenInclude: la lección con sus preguntas y las opciones de cada pregunta.
        // AsSingleQuery: una sola consulta con JOIN (elegido a propósito; ver la explicación).
        var leccion = await _context.Lecciones
            .AsNoTracking()
            .Include(l => l.Preguntas)
                .ThenInclude(p => p.Opciones)
            .AsSingleQuery()
            .SingleOrDefaultAsync(l => l.Id == id);

        if (leccion is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "La lección no existe.");
        }

        return LeccionDetalleResponse.Desde(leccion);
    }
}