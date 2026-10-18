using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> entity)
    {
        entity.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
        entity.Property(m => m.Descripcion).HasMaxLength(500);
        entity.HasIndex(m => m.Orden).IsUnique();
    }
}