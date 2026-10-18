using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class LeccionConfiguration : IEntityTypeConfiguration<Leccion>
{
    public void Configure(EntityTypeBuilder<Leccion> entity)
    {
        entity.Property(l => l.Titulo).IsRequired().HasMaxLength(150);
        entity.Property(l => l.Contenido).IsRequired(); // sin largo máximo → "text" en Postgres

        // No puede haber dos lecciones con el mismo Orden dentro del mismo módulo.
        entity.HasIndex(l => new { l.ModuloId, l.Orden }).IsUnique();

        entity.HasOne(l => l.Modulo)
              .WithMany(m => m.Lecciones)
              .HasForeignKey(l => l.ModuloId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}