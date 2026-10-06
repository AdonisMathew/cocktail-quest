using Microsoft.EntityFrameworkCore;
using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Data;

public class CocktailQuestContext : DbContext
{
	public CocktailQuestContext(DbContextOptions<CocktailQuestContext> options)
		: base(options)
	{
	}

	// Cada DbSet representa una tabla. "=> Set<Usuario>()" evita el warning de nullable (CS8618).
	public DbSet<Usuario> Usuarios => Set<Usuario>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Usuario>(entity =>
		{
			// Email y NombreUsuario no se pueden repetir entre usuarios
			// (decisión pendiente de docs/database-design.md).
			entity.HasIndex(u => u.Email).IsUnique();
			entity.HasIndex(u => u.NombreUsuario).IsUnique();

			entity.Property(u => u.Email)
				.IsRequired()
				.HasMaxLength(256);

			entity.Property(u => u.NombreUsuario)
				.IsRequired()
				.HasMaxLength(50);

			entity.Property(u => u.PasswordHash)
				.IsRequired();
		});
	}
}
