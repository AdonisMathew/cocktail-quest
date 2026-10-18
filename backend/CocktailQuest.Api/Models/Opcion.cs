
namespace CocktailQuest.Api.Models;

public class Opcion
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public bool EsCorrecta { get; set; }

    public Pregunta Pregunta { get; set; } = null!;
}