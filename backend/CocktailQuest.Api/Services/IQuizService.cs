using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Services;

/// <summary>
/// Lo que respondió el usuario en una pregunta: qué opción eligió.
/// </summary>
public record RespuestaEnviada(int PreguntaId, int OpcionId);

/// <summary>
/// La corrección de una pregunta: qué eligió el usuario, cuál era la correcta y si acertó.
/// </summary>
public record CorreccionPregunta(int PreguntaId, int OpcionElegidaId, int OpcionCorrectaId, bool EsCorrecta);

/// <summary>
/// El resultado de corregir un quiz completo.
/// </summary>
public record ResultadoQuiz(
    int Correctas,
    int Total,
    int Puntaje,
    bool Aprobado,
    IReadOnlyList<CorreccionPregunta> Detalle);

/// <summary>
/// Corrige los quizzes en el servidor. No accede a la base: trabaja con la lección ya cargada.
/// </summary>
public interface IQuizService
{
    /// <summary>
    /// Devuelve los problemas que tienen las respuestas. Si la lista está vacía, son válidas.
    /// </summary>
    IReadOnlyList<string> Validar(Leccion leccion, IReadOnlyCollection<RespuestaEnviada> respuestas);

    /// <summary>
    /// Corrige respuestas ya validadas. Si no son válidas, lanza ArgumentException.
    /// </summary>
    ResultadoQuiz Corregir(Leccion leccion, IReadOnlyCollection<RespuestaEnviada> respuestas);
}