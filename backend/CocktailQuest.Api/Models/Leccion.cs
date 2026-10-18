
namespace CocktailQuest.Api.Models;

public class Leccion
{
    public int Id { get; set; }
    public int ModuloId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int XpRecompensa { get; set; }

    public Modulo Modulo { get; set; } = null!;
    public List<Pregunta> Preguntas { get; set; } = new();
}