using CocktailQuest.Api.Models;
using CocktailQuest.Api.Services;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// El resultado de rendir un quiz: el puntaje de este intento y cómo quedó el progreso en la lección.
/// </summary>
public class ResultadoQuizResponse
{
    public int IntentoId { get; set; }
    public int Correctas { get; set; }
    public int Total { get; set; }

    /// <summary>Puntaje de este intento, de 0 a 100.</summary>
    public int Puntaje { get; set; }

    /// <summary>Si este intento llegó al puntaje mínimo para aprobar.</summary>
    public bool Aprobado { get; set; }

    /// <summary>Si la lección está completada (alcanza con haberla aprobado alguna vez).</summary>
    public bool LeccionCompletada { get; set; }

    /// <summary>El mejor puntaje del usuario en esta lección, contando este intento.</summary>
    public int MejorPuntaje { get; set; }

    public List<CorreccionPreguntaResponse> Preguntas { get; set; } = new();

    public static ResultadoQuizResponse Desde(int intentoId, ResultadoQuiz resultado, ProgresoLeccion progreso) => new()
    {
        IntentoId = intentoId,
        Correctas = resultado.Correctas,
        Total = resultado.Total,
        Puntaje = resultado.Puntaje,
        Aprobado = resultado.Aprobado,
        LeccionCompletada = progreso.Completada,
        MejorPuntaje = progreso.MejorPuntaje,
        Preguntas = resultado.Detalle
            .Select(d => CorreccionPreguntaResponse.Desde(d))
            .ToList()
    };
}