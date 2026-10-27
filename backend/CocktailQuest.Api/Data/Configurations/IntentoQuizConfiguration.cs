using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class IntentoQuizConfiguration : IEntityTypeConfiguration<IntentoQuiz>
{
    public void Configure(EntityTypeBuilder<IntentoQuiz> entity)
    {
        // Si se borra el usuario, se borran sus intentos: son datos suyos.
        entity.HasOne(i => i.Usuario)
              .WithMany()
              .HasForeignKey(i => i.UsuarioId)
              .OnDelete(DeleteBehavior.Cascade);

        // Una lección con intentos NO se puede borrar: protege el historial.
        entity.HasOne(i => i.Leccion)
              .WithMany()
              .HasForeignKey(i => i.LeccionId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}