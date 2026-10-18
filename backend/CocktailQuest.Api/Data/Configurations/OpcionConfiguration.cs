using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class OpcionConfiguration : IEntityTypeConfiguration<Opcion>
{
    public void Configure(EntityTypeBuilder<Opcion> entity)
    {
        entity.Property(o => o.Texto).IsRequired().HasMaxLength(200);

        entity.HasOne(o => o.Pregunta)
              .WithMany(p => p.Opciones)
              .HasForeignKey(o => o.PreguntaId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}