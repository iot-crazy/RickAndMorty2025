using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RickAndMorty.DB.Models;

namespace RickAndMorty.DB.ModelBuilders;

public class CharacterBuilder : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> entity)
    {
        entity.ToTable("Characters").HasKey(e => e.Id);

        /* Properties */
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.Status);
        entity.Property(e => e.Type);
        entity.Property(e => e.Gender);
        entity.Property(e => e.Image);
        entity.Property(e => e.Created);

        /* Relationships */
        entity.HasOne(x => x.Origin).WithMany().HasForeignKey(x => x.OriginId).OnDelete(DeleteBehavior.NoAction);
        entity.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.NoAction);
    }
}
