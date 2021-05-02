using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fairTeams.DemoHandling
{
    public class ShareCodeRepository : DbContext
    {
        private readonly ILogger<ShareCodeRepository> myLogger;
        private const int myMaximumDownloadAttempts = 10;
        public DbSet<ShareCode> ShareCodes { get; set; }

        public ShareCodeRepository(DbContextOptions<ShareCodeRepository> options, ILogger<ShareCodeRepository> logger) : base(options)
        { 
            myLogger = logger;
        }

        public void AddNew(List<ShareCode> codes)
        {
            foreach (var code in codes)
            {
                if (!ShareCodes.Contains(code))
                {
                    ShareCodes.Add(code);
                }
            }

            SaveChanges();
        }

        public IList<ShareCode> GetBatch(int count)
        {
            var orderedCodes = ShareCodes.AsEnumerable().OrderBy(x => x.EarliestRetry);
            if (!orderedCodes.Any())
            {
                return new List<ShareCode>();
            }

            var batch = orderedCodes.Take(count).ToList();
            foreach (var code in batch)
            {
                _ = IncrementDownloadAttemptCount(code);
            }

            return batch;
        }

        public IList<ShareCode> GetRetrieableBatch(int count)
        {
            var orderedRetrieableCodes = ShareCodes.AsEnumerable().OrderBy(x => x.EarliestRetry).Where(x => x.EarliestRetry <= DateTime.UtcNow);
            if (!orderedRetrieableCodes.Any())
            {
                return new List<ShareCode>();
            }

            var batch = orderedRetrieableCodes.Take(count).ToList();
            foreach (var code in batch)
            {
                _ = IncrementDownloadAttemptCount(code);
            }

            return batch;
        }

        private ShareCode IncrementDownloadAttemptCount(ShareCode code)
        {
            var dbShareCode = ShareCodes.AsEnumerable().Single(x => x.Equals(code));
            if (code.DownloadAttempt != dbShareCode.DownloadAttempt)
            {
                throw new ArgumentException($"The download attempt count for sharecode {code.Code} differs between the provided instance ({code.DownloadAttempt}) and the db {dbShareCode.DownloadAttempt}");
            }

            dbShareCode.IncrementDownloadAttempts();

            if (dbShareCode.DownloadAttempt >= myMaximumDownloadAttempts)
            {
                myLogger.LogInformation($"Share code {dbShareCode.Code} reached maximum number of attempts. Removing...");
                ShareCodes.Remove(dbShareCode);
            }

            SaveChanges();

            return dbShareCode;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShareCode>()
                .HasKey(m => m.Code);
        }
    }
}
