
namespace CocktailQuest.Api.Models;

public class Modulo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }

    public List<Leccion> Lecciones { get; set; } = new();
}