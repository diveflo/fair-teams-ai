using Microsoft.EntityFrameworkCore;

namespace fairTeams.Core
{
    public class MatchRepository : DbContext
    {
        public DbSet<Match> Matches { get; set; }

        public MatchRepository(DbContextOptions<MatchRepository> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Demo);
            modelBuilder.Entity<Match>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Match>()
                .HasMany(m => m.PlayerResults);
            modelBuilder.Entity<Demo>()
                .HasOne(d => d.GameRequest);
            modelBuilder.Entity<GameRequest>()
                .HasKey(g => g.MatchId);
        }
    }
}
