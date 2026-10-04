namespace CoctelIQ.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int Xp { get; set; } = 0;
    public int Nivel { get; set; } = 1;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}