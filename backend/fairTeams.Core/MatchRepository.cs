using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        public MatchRepository(DbContextOptions<MatchRepository> options) : this(options, UnitTestLoggerCreator.CreateUnitTestLogger<MatchRepository>()) { }

        public IList<Match> AddMatchesAndSave(IEnumerable<Match> newMatches)
        {
            var successfullyAddedMatches = new List<Match>();

            if (newMatches.Any())
            {
                Matches.Load();
                foreach (var match in newMatches)
                {
                    var alreadyExists = Matches.Any(x => x.Id == match.Id);
                    if (alreadyExists)
                    {
                        myLogger.LogWarning($"Match with id: {match.Id} already exists in repository.");
                        continue;
                    }

                    try
                    {
                        Matches.Add(match);
                        SaveChanges();
                        successfullyAddedMatches.Add(match);
                    }
                    catch (DbUpdateException e)
                    {
                        var innerSqlException = e.InnerException as SqliteException;

                        var isAlreadyAdded = innerSqlException.SqliteErrorCode == 19;
                        if (isAlreadyAdded)
                        {
                            myLogger.LogWarning($"Match with id: {match.Id} already exists in repository.");
                            continue;
                        }

                        throw;
                    }
                    catch (Exception e)
                    {
                        myLogger.LogCritical($"Unexpected exception: {e.Message} while trying to save match for ShareCode {match.Demo.ShareCode}");
                    }
                }
            }

            return successfullyAddedMatches;
        }

        public IList<MatchStatistics> GetAllMatchStatisticsForSteamId(long steamId)
        {
            Matches.Load();
            var allMatchstatistics = Matches.SelectMany(x => x.PlayerResults);
            var hasMatchesForSteamId = allMatchstatistics.Any(x => x.SteamID == steamId);
            if (!hasMatchesForSteamId)
            {
                throw new NoMatchstatisticsFoundException(steamId);
            }

            return allMatchstatistics.Where(x => x.SteamID == steamId).ToList();
        }

        public IList<MatchStatistics> GetLastMatchStatisticsForSteamId(long steamId, int numberOfMatchStatistics)
        {
            Matches.Load();
            var allMatchStatisticsOrderedByDate = Matches.OrderByDescending(x => x.Date).SelectMany(x => x.PlayerResults).ToList();
            var hasMatchesForSteamId = allMatchStatisticsOrderedByDate.Any(x => x.SteamID == steamId);
            if (!hasMatchesForSteamId)
            {
                throw new NoMatchstatisticsFoundException(steamId);
            }

            return allMatchStatisticsOrderedByDate.Where(x => x.SteamID == steamId).Take(numberOfMatchStatistics).ToList();
        }

        public MatchStatistics GetLatestMatchStatisticForSteamId(long steamId)
        {
            Matches.Load();
            var allMatchStatisticsOrderedByDate = Matches.OrderByDescending(x => x.Date).SelectMany(x => x.PlayerResults).ToList();
            var hasMatchesForSteamId = allMatchStatisticsOrderedByDate.Any(x => x.SteamID == steamId);
            if (!hasMatchesForSteamId)
            {
                throw new NoMatchstatisticsFoundException(steamId);
            }

            return allMatchStatisticsOrderedByDate.First(x => x.SteamID == steamId);
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
