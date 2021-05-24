using fairTeams.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public sealed class ShareCodeRepositoryTests : IDisposable
    {
        private readonly ShareCodeRepository myShareCodeRepository;

        public ShareCodeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ShareCodeRepository>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            myShareCodeRepository = new ShareCodeRepository(options, UnitTestLoggerCreator.CreateUnitTestLogger<ShareCodeRepository>());
        }

        [Fact]
        public void AddNew_EmptyRepository_IsAdded()
        {
            var shareCode = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM");

            myShareCodeRepository.AddNew(new List<ShareCode> { shareCode });

            Assert.Contains(shareCode, myShareCodeRepository.ShareCodes.AsEnumerable());
        }

        [Fact]
        public void AddNew_ShareCodeDoesNotExistYet_IsAdded()
        {
            var existingShareCode = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM");
            myShareCodeRepository.ShareCodes.Add(existingShareCode);
            myShareCodeRepository.SaveChanges();

            var newShareCode = new ShareCode("CSGO-b7UOr-F4sao-znyvb-3Q3HM-tJpxA");
            myShareCodeRepository.AddNew(new List<ShareCode> { newShareCode });

            Assert.Contains(newShareCode, myShareCodeRepository.ShareCodes.AsEnumerable());
        }

        [Fact]
        public void GetBatch_RepositoryHasEnough_RequestedCountReturned()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-b7UOr-F4sao-znyvb-3Q3HM-tJpxA"));
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-TZCze-SZnCt-kyGwE-GQ4tv-VCZQM"));
            myShareCodeRepository.SaveChanges();

            var batch = myShareCodeRepository.GetBatch(2);

            Assert.True(batch.Count == 2);
        }

        [Fact]
        public void GetBatch_RequestSingleForFirstTime_DownloadAttemptCountIsOne()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();

            var code = myShareCodeRepository.GetBatch(1).Single();

            Assert.Equal(1, code.DownloadAttempt);
        }

        [Fact]
        public void GetBatch_RepositoryHasSingleEntryRequestSingleForSecondTime_CodeIsTheSame()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();
            var code = myShareCodeRepository.GetBatch(1).Single();

            var secondRequestCode = myShareCodeRepository.GetBatch(1).Single();

            Assert.Equal("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM", code.Code);
            Assert.Equal("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM", secondRequestCode.Code);
        }

        [Fact]
        public void GetBatch_RequestSingleForSecondTime_DownloadAttemptCountIsTwo()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();
            _ = myShareCodeRepository.GetBatch(1).Single();

            var secondRequestCode = myShareCodeRepository.GetBatch(1).Single();

            Assert.Equal(2, secondRequestCode.DownloadAttempt);
        }

        [Fact]
        public void GetBatch_RequestSingleForEleventhTime_EmptyList()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();

            for (var i = 0; i < 10; i++)
            {
                _ = myShareCodeRepository.GetBatch(1);
            }

            var code = myShareCodeRepository.GetBatch(1);

            Assert.Empty(code);
        }

        [Fact]
        public void GetBatch_RepositoryHasTwoEntriesRequestTwoTimes_SecondCallReturnsSecondEntry()
        {
            var first = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM");
            var second = new ShareCode("CSGO-b7UOr-F4sao-znyvb-3Q3HM-tJpxA");
            myShareCodeRepository.ShareCodes.Add(first);
            myShareCodeRepository.ShareCodes.Add(second);
            myShareCodeRepository.SaveChanges();
            _ = myShareCodeRepository.GetBatch(1).Single();

            var secondCall = myShareCodeRepository.GetBatch(1).Single();

            Assert.Equal(second.Code, secondCall.Code);
        }

        [Fact]
        public void GetBatch_RepositoryHasFourEntriesAlreadyRequestedOneAfterTheOtherSubsequentRequestBatchOfTwo_FirstTwo()
        {
            var first = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM");
            var second = new ShareCode("CSGO-b7UOr-F4sao-znyvb-3Q3HM-tJpxA");
            var third = new ShareCode("CSGO-TZCze-SZnCt-kyGwE-GQ4tv-VCZQM");
            var fourth = new ShareCode("CSGO-8uiqf-UJHPv-WynJK-yhHQA-8xLDR");
            myShareCodeRepository.ShareCodes.Add(first);
            myShareCodeRepository.ShareCodes.Add(second);
            myShareCodeRepository.ShareCodes.Add(third);
            myShareCodeRepository.ShareCodes.Add(fourth);
            myShareCodeRepository.SaveChanges();

            _ = myShareCodeRepository.GetBatch(1).Single();
            _ = myShareCodeRepository.GetBatch(1).Single();
            _ = myShareCodeRepository.GetBatch(1).Single();
            _ = myShareCodeRepository.GetBatch(1).Single();

            var batchRequest = myShareCodeRepository.GetBatch(2);

            Assert.Equal(batchRequest.ElementAt(0).Code, first.Code);
            Assert.Equal(batchRequest.ElementAt(1).Code, second.Code);
        }

        [Fact]
        public void GetBatch_RequestSingle_EarliestRetryRoughly30Minutes()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();

            var code = myShareCodeRepository.GetBatch(1).Single();

            Assert.InRange(code.EarliestRetry, DateTime.UtcNow + TimeSpan.FromMinutes(29), DateTime.UtcNow + TimeSpan.FromMinutes(31));
        }

        [Fact]
        public void GetBatch_RequestSingleSecondTime_EarliestRetryIncreased()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();
            var earliestRetry = myShareCodeRepository.GetBatch(1).Single().EarliestRetry;

            var secondRequest = myShareCodeRepository.GetBatch(1).Single();

            Assert.True(secondRequest.EarliestRetry > earliestRetry);
        }

        [Fact]
        public void GetRetrieableBatch_RequestSingleBeforeEarliestRetry_EmptyList()
        {
            myShareCodeRepository.ShareCodes.Add(new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM"));
            myShareCodeRepository.SaveChanges();
            _ = myShareCodeRepository.GetBatch(1);

            var secondRequest = myShareCodeRepository.GetRetrieableBatch(1);

            Assert.Empty(secondRequest);
        }

        [Fact]
        public void GetRetrieableBatch_RequestSingleAfterEarliestRetry_ReturnsCode()
        {
            var codeWithZeroRetryCooldown = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM", TimeSpan.Zero);
            myShareCodeRepository.ShareCodes.Add(codeWithZeroRetryCooldown);
            myShareCodeRepository.SaveChanges();
            _ = myShareCodeRepository.GetBatch(1);

            var secondRequest = myShareCodeRepository.GetRetrieableBatch(1).Single();

            Assert.Equal(codeWithZeroRetryCooldown.Code, secondRequest.Code);
        }

        [Fact]
        public void GetRetrieableBatch_RepositoryHasOneEntryBeforeAndOneAfterEarliestRetryRequestTwo_ReturnsOneRetrieableEntry()
        {
            var codeWithRegularCooldown = new ShareCode("CSGO-b7UOr-F4sao-znyvb-3Q3HM-tJpxA");
            var codeWithZeroRetryCooldown = new ShareCode("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM", TimeSpan.Zero);
            myShareCodeRepository.ShareCodes.Add(codeWithRegularCooldown);
            myShareCodeRepository.ShareCodes.Add(codeWithZeroRetryCooldown);
            myShareCodeRepository.SaveChanges();
            _ = myShareCodeRepository.GetBatch(2);

            var secondRequest = myShareCodeRepository.GetRetrieableBatch(2);

            Assert.Single(secondRequest);
            Assert.Equal(codeWithZeroRetryCooldown.Code, secondRequest.Single().Code);
        }

        public void Dispose()
        {
            myShareCodeRepository.Database.EnsureDeleted();
            myShareCodeRepository.Dispose();
        }
    }
}
