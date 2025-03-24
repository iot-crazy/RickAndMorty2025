using Microsoft.EntityFrameworkCore;
using RickAndMorty.DB.Models;

namespace RickAndMorty.DB;

public class RickAndMortyContext(DbContextOptions<RickAndMortyContext> options) : DbContext(options)
{
    public DbSet<Character> Characters { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Location> Locations { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RickAndMortyContext).Assembly);
    }
}
