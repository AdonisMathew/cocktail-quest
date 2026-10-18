
namespace CocktailQuest.Api.Models;

public class Pregunta
{
    public int Id { get; set; }
    public int LeccionId { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public TipoPregunta TipoPregunta { get; set; } = TipoPregunta.OpcionMultiple;

    public Leccion Leccion { get; set; } = null!;
    public List<Opcion> Opciones { get; set; } = new();
}