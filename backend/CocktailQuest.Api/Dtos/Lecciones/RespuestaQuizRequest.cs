namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// La opción que el usuario eligió para una pregunta.
/// </summary>
public class RespuestaQuizRequest
{
    /// <summary>Id de la pregunta.</summary>
    /// <example>1</example>
    public int PreguntaId { get; set; }

    /// <summary>Id de la opción elegida.</summary>
    /// <example>2</example>
    public int OpcionId { get; set; }
}