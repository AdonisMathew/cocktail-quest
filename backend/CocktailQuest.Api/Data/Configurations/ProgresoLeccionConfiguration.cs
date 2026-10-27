using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class ProgresoLeccionConfiguration : IEntityTypeConfiguration<ProgresoLeccion>
{
    public void Configure(EntityTypeBuilder<ProgresoLeccion> entity)
    {
        // Un solo registro de progreso por usuario y lección.
        entity.HasIndex(p => new { p.UsuarioId, p.LeccionId }).IsUnique();

        entity.HasOne(p => p.Usuario)
              .WithMany()
              .HasForeignKey(p => p.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(p => p.Leccion)
              .WithMany()
              .HasForeignKey(p => p.LeccionId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}