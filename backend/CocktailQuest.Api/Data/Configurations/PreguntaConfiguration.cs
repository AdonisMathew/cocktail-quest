using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class PreguntaConfiguration : IEntityTypeConfiguration<Pregunta>
{
    public void Configure(EntityTypeBuilder<Pregunta> entity)
    {
        entity.Property(p => p.Enunciado).IsRequired().HasMaxLength(500);

        // Guarda "OpcionMultiple" en vez de 0: legible en la base y no se rompe si reordenás el enum.
        entity.Property(p => p.TipoPregunta).HasConversion<string>().HasMaxLength(30);

        entity.HasOne(p => p.Leccion)
              .WithMany(l => l.Preguntas)
              .HasForeignKey(p => p.LeccionId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}