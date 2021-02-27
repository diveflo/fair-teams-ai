using fairTeams.Core;
using fairTeams.DemoAnalyzer;
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
        private const int myEveryMinutesToTriggerProcessing = 30;
        private Timer myTimer;

        public MatchMakingDemoCollector(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            myScopeFactory = scopeFactory;
            myLoggerFactory = loggerFactory;
            myLogger = loggerFactory.CreateLogger<MatchMakingDemoCollector>();
        }

        public MatchMakingDemoCollector(IServiceScopeFactory scopeFactory) : this(scopeFactory, UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation($"MatchMakingDemoCollector timed hosted service started (trigger interval: {myEveryMinutesToTriggerProcessing} minutes)");
            myTimer = new Timer(ProcessNewMatches, null, TimeSpan.Zero, TimeSpan.FromMinutes(myEveryMinutesToTriggerProcessing));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("MatchMakingDemoCollector timed hosted service is stopping");
            myTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void ProcessNewMatches(object state)
        {
            using var scope = myScopeFactory.CreateScope();
            var shareCodeRepository = scope.ServiceProvider.GetRequiredService<ShareCodeRepository>();
            var newSharingCodes = shareCodeRepository.GetRetrieableBatch(5);

            if (!newSharingCodes.Any())
            {
                return;
            }

            myLogger.LogDebug($"Retrieved {newSharingCodes.Count} new sharing codes: {string.Join(", ", newSharingCodes.Select(x => x.Code))}");
            var successfullyDownloadedSharingCodes = new List<ShareCode>();
            var newMatches = new List<Match>();
            var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);
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

                successfullyDownloadedSharingCodes.Add(sharingCode);
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

                UpdateRanksForPlayers(demoReader.Match, gameCoordinatorClient);
            }

            myLogger.LogDebug($"Downloaded and analyzed {newMatches.Count} new matches (from {newSharingCodes.Count} new sharing codes).");
            myLogger.LogTrace($"Getting match repository to save {newMatches.Count} new matches.");
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();
            matchRepository.AddMatchesAndSave(newMatches);

            foreach (var successfullyDownloadedSharingCode in successfullyDownloadedSharingCodes)
            {
                shareCodeRepository.Remove(successfullyDownloadedSharingCode);
            }

            shareCodeRepository.SaveChanges();
        }

        private void BackupDemo(Demo demo, DemoBackuper backuper)
        {
            try
            {
                backuper.BackupDemoFile(demo);
            }
            catch (Exception)
            {
                myLogger.LogWarning($"Backing up the downloaded demo file ({demo.FilePath}) failed.");
            }
        }

        private void UpdateRanksForPlayers(Match match, GameCoordinatorClient gameCoordinatorClient)
        {
            using var scope = myScopeFactory.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<SteamUserRepository>();

            var knownPlayers = match.PlayerResults.Select(x => x.SteamID).Where(x => userRepository.SteamUsers.ToList().Select(y => y.SteamID).Contains(x));

            foreach (var steamId in knownPlayers)
            {
                try
                {
                    var rank = gameCoordinatorClient.GetRank(steamId);
                    myLogger.LogTrace($"Got rank {rank} for steam id: {steamId}");
                    userRepository.SteamUsers.Find(steamId).Rank = rank;
                }
                catch (GameCoordinatorException)
                {
                    myLogger.LogWarning($"Couldn't get rank for steam id {steamId}");
                    continue;
                }
            }

            userRepository.SaveChanges();
        }
    }
}
