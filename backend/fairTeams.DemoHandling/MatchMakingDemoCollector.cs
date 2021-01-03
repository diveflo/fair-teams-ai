using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using fairTeams.Steamworks;
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
    public class MatchMakingDemoCollector : IHostedService
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
            myLogger.LogInformation("MatchMakingDemoCollector timed hosted service started");
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
            var newSharingCodes = GetNewSharingCodes().Result;
            myLogger.LogInformation($"Retrieved {newSharingCodes.Count} new sharing codes: {string.Join(", ", newSharingCodes)}");
            var newMatches = new List<Match>();

            foreach (var sharingCode in newSharingCodes)
            {
                var gameRequest = ShareCode.Decode(sharingCode);
                myLogger.LogTrace($"Decoded sharing code {sharingCode} into Request with Match ID: {gameRequest.MatchId}, Outcome ID: {gameRequest.OutcomeId} and Token: {gameRequest.Token}");
                var demo = new Demo { ShareCode = sharingCode, GameRequest = gameRequest };
                Match match;

                var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);
                try
                {
                    match = gameCoordinatorClient.GetMatchInfo(demo);
                }
                catch (GameCoordinatorException)
                {
                    myLogger.LogWarning($"Couldn't get download url for sharing code {sharingCode}. See previous logs/exceptions for explanation. Continuing.");
                    continue;
                }

                string demoFilePath;
                try
                {
                    demoFilePath = DemoDownloader.DownloadAndDecompressDemo(match.Demo.DownloadURL);
                }
                catch (DemoDownloaderException exception)
                {
                    myLogger.LogWarning($"Demo downloading or decompressing failed: {exception.Message}");
                    continue;
                }

                myLogger.LogTrace($"Downloaded and decompressed demo file for sharing code {sharingCode}. Analyzing now.");
                match.Demo.FilePath = demoFilePath;

                using var demoReader = new DemoReader(match);
                try
                {
                    demoReader.ReadHeader();
                    demoReader.Read();
                }
                catch (DemoReaderException e)
                {
                    myLogger.LogWarning($"Analyzing demo for share code {sharingCode} failed: {e.Message}");
                    continue;
                }

                myLogger.LogTrace($"Finished analyzing demo file for sharing code {sharingCode}");
                newMatches.Add(demoReader.Match);
            }

            myLogger.LogInformation($"Downloaded and analyzed {newMatches.Count} new matches (from {newSharingCodes.Count} new sharing codes).");

            if (newMatches.Any())
            {
                using (var scope = myScopeFactory.CreateScope())
                {
                    myLogger.LogTrace($"Getting match repository to save {newMatches.Count} new matches.");
                    var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();
                    matchRepository.Matches.AddRange(newMatches);
                    myLogger.LogTrace("Saving new matches");
                    matchRepository.SaveChanges();
                }
            }
        }

        private async Task<List<string>> GetNewSharingCodes()
        {
            using var scope = myScopeFactory.CreateScope();
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();
            var userRepository = scope.ServiceProvider.GetRequiredService<SteamUserRepository>();

            var alreadyProcessedSharingCodes = matchRepository.Matches.Include("Demo").AsEnumerable().Select(x => x.Demo.ShareCode);
            var newSharingCodes = new List<string>();

            var steamUsers = userRepository.SteamUsers.ToList();

            foreach (var user in steamUsers)
            {
                var sharingCode = await SteamworksApi.GetNextMatchSharingCode(user.SteamID.ToString(), user.AuthenticationCode, user.LastSharingCode);
                if (sharingCode.Equals("n/a") || sharingCode.Equals(user.LastSharingCode))
                {
                    myLogger.LogTrace($"No new sharing code for Steam ID: {user.SteamID}");
                    continue;
                }

                myLogger.LogTrace($"New sharing code {sharingCode} for Steam ID: {user.SteamID}");

                user.LastSharingCode = sharingCode;

                if (newSharingCodes.Contains(sharingCode) || alreadyProcessedSharingCodes.Contains(sharingCode))
                {
                    continue;
                }

                newSharingCodes.Add(sharingCode);
            }

            userRepository.SaveChanges();

            return newSharingCodes;
        }
    }
}
