namespace CocktailQuest.Api.Models;

/// <summary>
/// Cada vez que un usuario rinde el quiz de una lección. Se guardan todos: es el historial.
/// </summary>
public class IntentoQuiz
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int LeccionId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int Puntaje { get; set; }      // De 0 a 100.
    public bool Aprobado { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Leccion Leccion { get; set; } = null!;
    public List<RespuestaUsuario> Respuestas { get; set; } = new();
}