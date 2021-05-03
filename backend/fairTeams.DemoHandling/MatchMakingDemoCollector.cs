using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public sealed class MatchMakingDemoCollector : IHostedService
    {
        private readonly IServiceScopeFactory myScopeFactory;
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<MatchMakingDemoCollector> myLogger;
        private const int myMatchMakingCollectionTriggerInMinutes = 30;
        private const int myMatchMakingCollectorTriggerOffsetInMinutes = 0;
        private const int myRankCheckerTriggerInMinutes = 360;
        private const int myRankCheckerTriggerOffsetInMinutes = 375;
        private Timer myMatchMakingCollectionSchedule;
        private Timer myRankCheckerSchedule;

        public MatchMakingDemoCollector(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            myScopeFactory = scopeFactory;
            myLoggerFactory = loggerFactory;
            myLogger = loggerFactory.CreateLogger<MatchMakingDemoCollector>();
        }

        public MatchMakingDemoCollector(IServiceScopeFactory scopeFactory) : this(scopeFactory, UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation($"MatchMakingDemoCollector timed hosted service started. {System.Environment.NewLine}" +
                $"Match making collector schedule: {myMatchMakingCollectorTriggerOffsetInMinutes} min offset, trigger every {myMatchMakingCollectionTriggerInMinutes} min {Environment.NewLine}" +
                $"Rank checker schedule: {myRankCheckerTriggerOffsetInMinutes} min offset, trigger every {myRankCheckerTriggerInMinutes}");

            myMatchMakingCollectionSchedule = new Timer(ProcessNewMatches, null, TimeSpan.FromMinutes(myMatchMakingCollectorTriggerOffsetInMinutes), TimeSpan.FromMinutes(myMatchMakingCollectionTriggerInMinutes));
            myRankCheckerSchedule = new Timer(CheckForRankChanges, null, TimeSpan.FromMinutes(myRankCheckerTriggerOffsetInMinutes), TimeSpan.FromMinutes(myRankCheckerTriggerInMinutes));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("MatchMakingDemoCollector timed hosted service is stopping");
            myMatchMakingCollectionSchedule?.Change(Timeout.Infinite, 0);
            myRankCheckerSchedule?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void ProcessNewMatches(object state)
        {
            var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);
            try
            {
                gameCoordinatorClient.ConnectAndLogin();
                ProcessNewMatches(gameCoordinatorClient);
            }
            catch (GameCoordinatorException)
            {
                myLogger.LogWarning("Error while trying to process new matches");
            }
            finally
            {
                gameCoordinatorClient.Dispose();
            }
        }

        private void ProcessNewMatches(GameCoordinatorClient gameCoordinatorClient)
        {
            using var scope = myScopeFactory.CreateScope();
            var shareCodeRepository = scope.ServiceProvider.GetRequiredService<ShareCodeRepository>();
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();

            PruneShareCodeRepository(shareCodeRepository, matchRepository);

            var newSharingCodes = shareCodeRepository.GetRetrieableBatch(5);

            if (!newSharingCodes.Any())
            {
                return;
            }

            myLogger.LogDebug($"Retrieved {newSharingCodes.Count} new sharing codes: {string.Join(", ", newSharingCodes.Select(x => x.Code))}");
            var newMatches = new List<Match>();
            var demoDownloader = new DemoDownloader(myLoggerFactory.CreateLogger<DemoDownloader>());
            var demoBackuper = new DemoBackuper(myLoggerFactory.CreateLogger<DemoBackuper>());

            foreach (var sharingCode in newSharingCodes)
            {
                var gameRequest = ShareCodeDecoder.Decode(sharingCode.Code);
                myLogger.LogTrace($"Decoded sharing code {sharingCode.Code} into Request with Match ID: {gameRequest.MatchId}, Outcome ID: {gameRequest.OutcomeId} and Token: {gameRequest.Token}");
                var demo = new Demo { ShareCode = sharingCode.Code, GameRequest = gameRequest };
                Match match;

                try
                {
                    match = gameCoordinatorClient.GetMatchInfo(demo);
                }
                catch (GameCoordinatorException)
                {
                    myLogger.LogWarning($"Couldn't get download url for sharing code {sharingCode.Code}. See previous logs/exceptions for explanation. Continuing.");
                    continue;
                }

                string demoFilePath;
                try
                {
                    demoFilePath = demoDownloader.DownloadAndDecompressDemo(match.Demo.DownloadURL);
                }
                catch (DemoDownloaderException exception)
                {
                    myLogger.LogWarning($"Demo downloading or decompressing failed: {exception.Message}");
                    continue;
                }

                myLogger.LogTrace($"Downloaded and decompressed demo file for sharing code {sharingCode.Code}");
                match.Demo.FilePath = demoFilePath;

                using var demoReader = new DemoReader(match, 0, 0);
                try
                {
                    demoReader.ReadHeader();
                    demoReader.Read();
                }
                catch (DemoReaderException e)
                {
                    myLogger.LogWarning($"Analyzing demo for share code {sharingCode.Code} failed: {e.Message}");
                    BackupDemo(demo, demoBackuper);
                    continue;
                }

                BackupDemo(demo, demoBackuper);

                myLogger.LogTrace($"Finished analyzing demo file for sharing code {sharingCode.Code}");
                newMatches.Add(demoReader.Match);
            }

            myLogger.LogDebug($"Downloaded and analyzed {newMatches.Count} new matches (from {newSharingCodes.Count} new sharing codes).");
            myLogger.LogTrace($"Getting match repository to save {newMatches.Count} new matches.");

            var successfullySavedMatches = matchRepository.AddMatchesAndSave(newMatches);

            shareCodeRepository.RemoveCodes(successfullySavedMatches.Select(x => x.Demo.ShareCode));

            UpdateRanksForPlayers(gameCoordinatorClient);
        }

        public void CheckForRankChanges(object state)
        {
            var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);

            try
            {
                gameCoordinatorClient.ConnectAndLogin();
                UpdateRanksForPlayers(gameCoordinatorClient);
            }
            catch (GameCoordinatorException)
            {
                myLogger.LogError("Error while trying to update ranks");
            }
            finally
            {
                gameCoordinatorClient.Dispose();
            }
        }

        private void PruneShareCodeRepository(ShareCodeRepository shareCodeRepository, MatchRepository matchRepository)
        {
            var alreadyProcessedSharingCodes = matchRepository.Matches.Include("Demo").AsEnumerable().Select(x => x.Demo.ShareCode);
            shareCodeRepository.RemoveCodes(alreadyProcessedSharingCodes);
        }

        private void BackupDemo(Demo demo, DemoBackuper backuper)
        {
            try
            {
                backuper.BackupDemoFile(demo);
            }
            catch (Exception)
            {
                myLogger.LogError($"Backing up the downloaded demo file ({demo.FilePath}) failed.");
            }
        }

        private void UpdateRanksForPlayers(GameCoordinatorClient gameCoordinatorClient)
        {
            using var scope = myScopeFactory.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<SteamUserRepository>();

            var steamUsers = userRepository.SteamUsers.AsEnumerable().ToList();
            myLogger.LogInformation($"Checking for rank changes of {steamUsers.Count} players");

            foreach (var user in steamUsers)
            {
                var steamId = user.SteamID;

                try
                {
                    var rank = gameCoordinatorClient.GetRank(steamId);
                    myLogger.LogTrace($"Got rank {rank} for steam id: {steamId}");
                    user.Rank = rank;
                    userRepository.SaveChanges();
                }
                catch (GameCoordinatorException)
                {
                    myLogger.LogWarning($"Couldn't get rank for steam id {steamId}");
                    continue;
                }
            }
        }
    }
}
