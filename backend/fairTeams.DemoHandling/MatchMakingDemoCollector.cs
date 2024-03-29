﻿using fairTeams.Core;
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
        private Timer myMatchMakingCollectionSchedule;

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
                $"Match making collector schedule: {myMatchMakingCollectorTriggerOffsetInMinutes} min offset, trigger every {myMatchMakingCollectionTriggerInMinutes} min {Environment.NewLine}");

            myMatchMakingCollectionSchedule = new Timer(ProcessNewMatches, null, TimeSpan.FromMinutes(myMatchMakingCollectorTriggerOffsetInMinutes), TimeSpan.FromMinutes(myMatchMakingCollectionTriggerInMinutes));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("MatchMakingDemoCollector timed hosted service is stopping");
            myMatchMakingCollectionSchedule?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void ProcessNewMatches(object state)
        {
            var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);

            using var scope = myScopeFactory.CreateScope();
            var shareCodeRepository = scope.ServiceProvider.GetRequiredService<ShareCodeRepository>();
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();

            PruneShareCodeRepository(shareCodeRepository, matchRepository);

            try
            {
                if (!shareCodeRepository.HasRetryableCodes())
                {
                    myLogger.LogInformation("No retryable share codes available right now");
                    return;
                }

                gameCoordinatorClient.ConnectAndLogin();

                ProcessNewMatches(gameCoordinatorClient, shareCodeRepository, matchRepository);
                Thread.Sleep(5000);
            }
            catch (GameCoordinatorException e)
            {
                myLogger.LogWarning($"Exception while trying to process new matches: {e.Message}");
            }
            finally
            {
                matchRepository.Dispose();
                shareCodeRepository.Dispose();
                scope.Dispose();
                gameCoordinatorClient.Dispose();
            }
        }

        private void ProcessNewMatches(GameCoordinatorClient gameCoordinatorClient, ShareCodeRepository shareCodeRepository, MatchRepository matchRepository)
        {
            var newSharingCodes = shareCodeRepository.GetRetryableBatch(5);

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
                var demo = CreateNewDemoForShareCode(sharingCode);

                try
                {
                    var match = gameCoordinatorClient.GetMatchInfo(demo);
                    myLogger.LogTrace($"Got match details for sharing code {sharingCode.Code}");

                    match.Demo.FilePath = demoDownloader.DownloadAndDecompressDemo(match.Demo.DownloadURL);
                    myLogger.LogTrace($"Downloaded and decompressed demo file for sharing code {sharingCode.Code}");

                    using var demoReader = new DemoReader(match, 0, 0);
                    match = demoReader.Parse();

                    myLogger.LogTrace($"Finished analyzing demo file for sharing code {sharingCode.Code}");
                    newMatches.Add(match);
                }
                catch (GameCoordinatorException)
                {
                    myLogger.LogWarning($"Couldn't get download url for sharing code {sharingCode.Code}. See previous logs/exceptions for explanation. Continuing.");
                    continue;
                }
                catch (DemoDownloaderException exception)
                {
                    myLogger.LogWarning($"Demo downloading or decompressing failed: {exception.Message} for sharing code {sharingCode.Code}.");
                    continue;
                }
                catch (DemoReaderException e)
                {
                    myLogger.LogWarning($"Analyzing demo for share code {sharingCode.Code} failed: {e.Message}");
                    demo.State = DemoState.ParseFailure;
                    continue;
                }
                finally
                {
                    TryBackupDemo(demo, demoBackuper);
                }
            }

            myLogger.LogDebug($"Downloaded and analyzed {newMatches.Count} new matches (from {newSharingCodes.Count} new sharing codes).");
            myLogger.LogTrace($"Saving {newMatches.Count} new matches to repository.");

            var successfullySavedMatches = matchRepository.AddMatchesAndSave(newMatches);

            shareCodeRepository.RemoveCodes(successfullySavedMatches.Select(x => x.Demo.ShareCode));
        }

        private static void PruneShareCodeRepository(ShareCodeRepository shareCodeRepository, MatchRepository matchRepository)
        {
            var alreadyProcessedSharingCodes = matchRepository.Matches.Include("Demo").AsEnumerable().Select(x => x.Demo.ShareCode);
            shareCodeRepository.RemoveCodes(alreadyProcessedSharingCodes);
        }

        private Demo CreateNewDemoForShareCode(ShareCode shareCode)
        {
            var gameRequest = ShareCodeDecoder.Decode(shareCode.Code);
            myLogger.LogTrace($"Decoded sharing code {shareCode.Code} into Request with Match ID: {gameRequest.MatchId}, Outcome ID: {gameRequest.OutcomeId} and Token: {gameRequest.Token}");
            return new Demo { ShareCode = shareCode.Code, GameRequest = gameRequest };
        }

        private void TryBackupDemo(Demo demo, DemoBackuper backuper)
        {
            if (string.IsNullOrEmpty(demo.FilePath))
            {
                myLogger.LogWarning($"No local file path for demo to backup given (share code: {demo.ShareCode}");
                return;
            }

            try
            {
                backuper.BackupDemoFile(demo);
            }
            catch (Exception)
            {
                myLogger.LogError($"Backing up the downloaded demo file ({demo.FilePath}) failed.");
            }
        }
    }
}
