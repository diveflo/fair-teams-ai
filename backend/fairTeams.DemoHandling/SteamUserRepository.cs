using Microsoft.EntityFrameworkCore;

namespace fairTeams.DemoHandling
{
    public class SteamUserRepository : DbContext
    {
        public DbSet<MatchMakingSteamUser> SteamUsers { get; set; }

        public SteamUserRepository(DbContextOptions<SteamUserRepository> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchMakingSteamUser>()
                .HasKey(m => m.SteamID);
        }
    }
}
