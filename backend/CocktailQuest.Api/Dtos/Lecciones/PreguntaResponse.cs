using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// Una pregunta del quiz con sus opciones (sin revelar la correcta).
/// </summary>
public class PreguntaResponse
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public TipoPregunta TipoPregunta { get; set; }
    public List<OpcionResponse> Opciones { get; set; } = new();

    public static PreguntaResponse Desde(Pregunta pregunta) => new()
    {
        Id = pregunta.Id,
        Enunciado = pregunta.Enunciado,
        TipoPregunta = pregunta.TipoPregunta,
        Opciones = pregunta.Opciones
            .OrderBy(o => o.Id)
            .Select(o => OpcionResponse.Desde(o))
            .ToList()
    };
}