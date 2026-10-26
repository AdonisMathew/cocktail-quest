namespace CocktailQuest.Api.Dtos.Modulos;

/// <summary>
/// Un módulo con el resumen de sus lecciones, en orden.
/// </summary>
public class ModuloResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }
    public List<LeccionResumenResponse> Lecciones { get; set; } = new();
}