using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RickAndMorty.DB.Models;

namespace RickAndMorty.DB.ModelBuilders;

public class LocationBuilder : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> entity)
    {
        entity.ToTable("Locations").HasKey(e => e.Id);

        /* Properties */
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.Dimension).IsRequired();
        entity.Property(e => e.Url).IsRequired();
        entity.Property(e => e.Created);
    }
}
