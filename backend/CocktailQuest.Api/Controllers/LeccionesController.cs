using System.Security.Claims;
using CocktailQuest.Api.Data;
using CocktailQuest.Api.Dtos.Lecciones;
using CocktailQuest.Api.Models;
using CocktailQuest.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

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
    private readonly IQuizService _quizService;

    public LeccionesController(CocktailQuestContext context, IQuizService quizService)
    {
        _context = context;
        _quizService = quizService;
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
        var leccion = await CargarLeccionConPreguntasAsync(id);

        if (leccion is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "La lección no existe.");
        }

        return LeccionDetalleResponse.Desde(leccion);
    }

    /// <summary>
    /// Rinde el quiz de una lección: lo corrige en el servidor y guarda el intento y el progreso.
    /// </summary>
    /// <param name="id">Id de la lección.</param>
    /// <param name="request">Una respuesta por cada pregunta de la lección.</param>
    /// <response code="200">El quiz corregido: puntaje, si aprobó, el progreso y la corrección de cada pregunta.</response>
    /// <response code="400">Las respuestas no son válidas: faltan preguntas, hay repetidas o no son de esta lección.</response>
    /// <response code="401">Falta el token o no es válido.</response>
    /// <response code="404">No existe una lección con ese id.</response>
    [HttpPost("{id:int}/quiz")]
    [ProducesResponseType(typeof(ResultadoQuizResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultadoQuizResponse>> RendirQuiz(int id, RendirQuizRequest request)
    {
        // El Id del usuario viaja en el claim "sub" del token (igual que en UsuariosController).
        if (!int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var usuarioId))
        {
            return Unauthorized();
        }

        var leccion = await CargarLeccionConPreguntasAsync(id);

        if (leccion is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "La lección no existe.");
        }

        var respuestas = request.Respuestas
            .Select(r => new RespuestaEnviada(r.PreguntaId, r.OpcionId))
            .ToList();

        // 1. Validar: si algo está mal, 400 con todos los problemas juntos.
        var errores = _quizService.Validar(leccion, respuestas);
        if (errores.Count > 0)
        {
            foreach (var error in errores)
            {
                ModelState.AddModelError(nameof(request.Respuestas), error);
            }

            return ValidationProblem(ModelState);
        }

        // 2. Corregir: lógica pura del servicio, sin base de datos.
        var resultado = _quizService.Corregir(leccion, respuestas);

        // 3. El intento, con una respuesta por pregunta.
        var intento = new IntentoQuiz
        {
            UsuarioId = usuarioId,
            LeccionId = leccion.Id,
            Puntaje = resultado.Puntaje,
            Aprobado = resultado.Aprobado,
            Respuestas = resultado.Detalle
                .Select(d => new RespuestaUsuario
                {
                    PreguntaId = d.PreguntaId,
                    OpcionSeleccionadaId = d.OpcionElegidaId,
                    EsCorrecta = d.EsCorrecta
                })
                .ToList()
        };
        _context.IntentosQuiz.Add(intento);

        // 4. El progreso: se queda con el mejor puntaje, y la lección queda
        //    completada la primera vez que se aprueba (después no se "descompleta").
        var progreso = await _context.ProgresosLeccion
            .SingleOrDefaultAsync(p => p.UsuarioId == usuarioId && p.LeccionId == leccion.Id);

        if (progreso is null)
        {
            progreso = new ProgresoLeccion { UsuarioId = usuarioId, LeccionId = leccion.Id };
            _context.ProgresosLeccion.Add(progreso);
        }

        progreso.MejorPuntaje = Math.Max(progreso.MejorPuntaje, resultado.Puntaje);

        if (resultado.Aprobado && !progreso.Completada)
        {
            progreso.Completada = true;
            progreso.FechaCompletado = DateTime.UtcNow;
        }

        // Un solo SaveChanges = una sola transacción: se guarda todo o no se guarda nada.
        await _context.SaveChangesAsync();

        return ResultadoQuizResponse.Desde(intento.Id, resultado, progreso);
    }

    /// <summary>
    /// La lección con sus preguntas y las opciones de cada una, sin seguimiento de cambios.
    /// </summary>
    private Task<Leccion?> CargarLeccionConPreguntasAsync(int id) =>
        _context.Lecciones
            .AsNoTracking()
            .Include(l => l.Preguntas)
                .ThenInclude(p => p.Opciones)
            .AsSingleQuery()
            .SingleOrDefaultAsync(l => l.Id == id);
}