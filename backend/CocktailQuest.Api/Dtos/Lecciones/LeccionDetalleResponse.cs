using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// Una lección completa: el contenido para leer y las preguntas del quiz.
/// </summary>
public class LeccionDetalleResponse
{
    public int Id { get; set; }
    public int ModuloId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int XpRecompensa { get; set; }
    public List<PreguntaResponse> Preguntas { get; set; } = new();

    public static LeccionDetalleResponse Desde(Leccion leccion) => new()
    {
        Id = leccion.Id,
        ModuloId = leccion.ModuloId,
        Titulo = leccion.Titulo,
        Contenido = leccion.Contenido,
        Orden = leccion.Orden,
        XpRecompensa = leccion.XpRecompensa,
        Preguntas = leccion.Preguntas
            .OrderBy(p => p.Id)
            .Select(p => PreguntaResponse.Desde(p))
            .ToList()
    };
}