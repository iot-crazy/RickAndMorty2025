using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RickAndMorty.DB.Models;

namespace RickAndMorty.DB.ModelBuilders;

public class CharacterEpisodeBuilder : IEntityTypeConfiguration<CharacterEpisode>
{
    public void Configure(EntityTypeBuilder<CharacterEpisode> entity)
    {
        entity.ToTable("Character_Episodes").HasKey(e => new { e.CharacterId, e.EpisodeId });

        entity.HasOne(x => x.Character)
             .WithMany(e => e.CharacterEpisodes)
             .HasForeignKey(fk => fk.CharacterId);

        entity.HasOne(x => x.Episode)
            .WithMany(e => e.CharacterEpisodes)
            .HasForeignKey(fk => fk.EpisodeId);
    }
}
