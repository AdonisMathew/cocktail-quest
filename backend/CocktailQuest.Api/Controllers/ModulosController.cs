using CocktailQuest.Api.Data;
using CocktailQuest.Api.Dtos.Modulos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CocktailQuest.Api.Controllers;

/// <summary>
/// Módulos de aprendizaje y sus lecciones. Todos los endpoints requieren token.
/// </summary>
[ApiController]
[Route("api/modulos")]
[Authorize]
[Produces("application/json")]
public class ModulosController : ControllerBase
{
    private readonly CocktailQuestContext _context;

    public ModulosController(CocktailQuestContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista los módulos en orden, cada uno con el resumen de sus lecciones.
    /// </summary>
    /// <response code="200">Módulos ordenados, con sus lecciones ordenadas.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ModuloResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ModuloResponse>>> Listar()
    {
        // Proyección con Select: EF arma un SELECT solo con las columnas que usa el DTO.
        // Por ejemplo, no trae el Contenido de cada lección, que es largo y acá no se muestra.
        var modulos = await _context.Modulos
            .OrderBy(m => m.Orden)
            .Select(m => new ModuloResponse
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Orden = m.Orden,
                Lecciones = m.Lecciones
                    .OrderBy(l => l.Orden)
                    .Select(l => new LeccionResumenResponse
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        Orden = l.Orden,
                        XpRecompensa = l.XpRecompensa
                    })
                    .ToList()
            })
            .ToListAsync();

        return modulos;
    }
}