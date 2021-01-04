using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace fairTeams.Core
{
    public class MatchRepository : DbContext
    {
        private readonly ILogger<MatchRepository> myLogger;
        public virtual DbSet<Match> Matches { get; set; }

        public MatchRepository(DbContextOptions<MatchRepository> options, ILogger<MatchRepository> logger) : base(options)
        {
            myLogger = logger;
        }

        public void AddMatchesAndSave(IEnumerable<Match> newMatches)
        {
            if (newMatches.Any())
            {
                foreach (var match in newMatches)
                {
                    try
                    {
                        Matches.Add(match);
                        SaveChanges();
                    }
                    catch (DbUpdateException e)
                    {
                        var innerSqlException = e.InnerException as SqliteException;
                        var isAlreadyAdded = innerSqlException.SqliteErrorCode == 19;
                        if (isAlreadyAdded)
                        {
                            myLogger.LogDebug($"Match with id: {match.Id} already exists in repository.");
                            continue;
                        }

                        throw;
                    }
                }
            }
        }

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
