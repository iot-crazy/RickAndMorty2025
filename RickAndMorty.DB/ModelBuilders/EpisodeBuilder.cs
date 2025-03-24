using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RickAndMorty.DB.Models;

namespace RickAndMorty.DB.ModelBuilders;

public class EpisodeBuilder : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> entity)
    {
        entity.ToTable("Episodes").HasKey(e => e.Id);

        /* Properties */
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.AirDate).IsRequired();
        entity.Property(e => e.Code).IsRequired();
        entity.Property(e => e.Url).IsRequired();
        entity.Property(e => e.Created);
    }
}
