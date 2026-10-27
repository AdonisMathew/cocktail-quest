namespace CocktailQuest.Api.Models;

/// <summary>
/// El estado de un usuario en una lección. Hay un solo registro por usuario y lección.
/// </summary>
public class ProgresoLeccion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int LeccionId { get; set; }
    public bool Completada { get; set; }
    public DateTime? FechaCompletado { get; set; }  // null hasta que la aprueba por primera vez.
    public int MejorPuntaje { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Leccion Leccion { get; set; } = null!;
}