using CocktailQuest.Api.Configuration;
using CocktailQuest.Api.Models;
using Microsoft.Extensions.Options;

namespace CocktailQuest.Api.Services;

public class QuizService : IQuizService
{
    private readonly QuizSettings _settings;

    public QuizService(IOptions<QuizSettings> options)
    {
        _settings = options.Value;
    }

    public IReadOnlyList<string> Validar(Leccion leccion, IReadOnlyCollection<RespuestaEnviada> respuestas)
    {
        var errores = new List<string>();

        if (leccion.Preguntas.Count == 0)
        {
            errores.Add("La lección no tiene preguntas.");
            return errores;
        }

        // Diccionario Id → pregunta, para buscar cada pregunta sin recorrer la lista entera.
        var preguntas = leccion.Preguntas.ToDictionary(p => p.Id);

        // 1. Preguntas respondidas más de una vez.
        var repetidas = respuestas
            .GroupBy(r => r.PreguntaId)
            .Where(grupo => grupo.Count() > 1)
            .Select(grupo => grupo.Key);

        foreach (var preguntaId in repetidas)
        {
            errores.Add($"La pregunta {preguntaId} está respondida más de una vez.");
        }

        // 2. Preguntas de otra lección y opciones de otra pregunta.
        foreach (var respuesta in respuestas)
        {
            if (!preguntas.TryGetValue(respuesta.PreguntaId, out var pregunta))
            {
                errores.Add($"La pregunta {respuesta.PreguntaId} no es de esta lección.");
            }
            else if (pregunta.Opciones.All(o => o.Id != respuesta.OpcionId))
            {
                errores.Add($"La opción {respuesta.OpcionId} no es de la pregunta {respuesta.PreguntaId}.");
            }
        }

        // 3. Preguntas sin responder.
        var respondidas = respuestas.Select(r => r.PreguntaId).ToHashSet();
        var faltantes = preguntas.Keys.Where(id => !respondidas.Contains(id)).Order().ToList();

        if (faltantes.Count > 0)
        {
            errores.Add($"Faltan responder las preguntas: {string.Join(", ", faltantes)}.");
        }

        return errores;
    }

    public ResultadoQuiz Corregir(Leccion leccion, IReadOnlyCollection<RespuestaEnviada> respuestas)
    {
        // Red de seguridad: nunca se corrige un quiz inválido, aunque alguien se olvide de validar antes.
        var errores = Validar(leccion, respuestas);
        if (errores.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errores), nameof(respuestas));
        }

        var elegidas = respuestas.ToDictionary(r => r.PreguntaId, r => r.OpcionId);

        var detalle = leccion.Preguntas
            .OrderBy(p => p.Id)
            .Select(p =>
            {
                var correctaId = p.Opciones.Single(o => o.EsCorrecta).Id;
                var elegidaId = elegidas[p.Id];
                return new CorreccionPregunta(p.Id, elegidaId, correctaId, elegidaId == correctaId);
            })
            .ToList();

        var correctas = detalle.Count(d => d.EsCorrecta);
        var puntaje = CalcularPuntaje(correctas, detalle.Count);

        // Se aprueba comparando el puntaje redondeado: el que el usuario ve en pantalla.
        var aprobado = puntaje >= _settings.PorcentajeAprobacion;

        return new ResultadoQuiz(correctas, detalle.Count, puntaje, aprobado, detalle);
    }

    /// <summary>
    /// Porcentaje de aciertos de 0 a 100, redondeado como en la escuela (el ,5 sube).
    /// </summary>
    public static int CalcularPuntaje(int correctas, int total)
    {
        if (total <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "Tiene que haber al menos una pregunta.");
        }

        return (int)Math.Round(correctas * 100.0 / total, MidpointRounding.AwayFromZero);
    }
}