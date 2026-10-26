namespace CocktailQuest.Api.Dtos.Modulos;

/// <summary>
/// Una lección dentro del listado de módulos: solo lo necesario para mostrarla en el mapa.
/// </summary>
public class LeccionResumenResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int XpRecompensa { get; set; }
}