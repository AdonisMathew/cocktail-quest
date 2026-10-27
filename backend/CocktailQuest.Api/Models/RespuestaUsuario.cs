namespace CocktailQuest.Api.Models;

/// <summary>
/// La opción que eligió el usuario para una pregunta, dentro de un intento.
/// </summary>
public class RespuestaUsuario
{
    public int Id { get; set; }
    public int IntentoQuizId { get; set; }
    public int PreguntaId { get; set; }
    public int OpcionSeleccionadaId { get; set; }
    public bool EsCorrecta { get; set; }  // Se guarda como quedó en ese momento.

    public IntentoQuiz IntentoQuiz { get; set; } = null!;
    public Pregunta Pregunta { get; set; } = null!;
    public Opcion OpcionSeleccionada { get; set; } = null!;
}