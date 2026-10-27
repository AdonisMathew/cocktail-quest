using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class RespuestaUsuarioConfiguration : IEntityTypeConfiguration<RespuestaUsuario>
{
    public void Configure(EntityTypeBuilder<RespuestaUsuario> entity)
    {
        // Las respuestas son parte del intento: si se borra el intento, se van con él.
        entity.HasOne(r => r.IntentoQuiz)
              .WithMany(i => i.Respuestas)
              .HasForeignKey(r => r.IntentoQuizId)
              .OnDelete(DeleteBehavior.Cascade);

        // Preguntas y opciones respondidas NO se pueden borrar: protege el historial.
        entity.HasOne(r => r.Pregunta)
              .WithMany()
              .HasForeignKey(r => r.PreguntaId)
              .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(r => r.OpcionSeleccionada)
              .WithMany()
              .HasForeignKey(r => r.OpcionSeleccionadaId)
              .OnDelete(DeleteBehavior.Restrict);

        // Dentro de un intento, cada pregunta se responde una sola vez.
        entity.HasIndex(r => new { r.IntentoQuizId, r.PreguntaId }).IsUnique();
    }
}