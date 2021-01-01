using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using fairTeams.Steamworks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public class MatchMakingDemoCollector
    {
        private readonly DbContextOptions<MatchRepository> myMatchRepositoryOptions;
        private readonly DbContextOptions<SteamUserRepository> mySteamUserRepositoryOptions;
        private readonly ILogger<MatchMakingDemoCollector> myLogger;

        public MatchMakingDemoCollector(DbContextOptions<MatchRepository> matchRepositoryOptions, DbContextOptions<SteamUserRepository> steamUserRepositoryOptions, ILogger<MatchMakingDemoCollector> logger)
        {
            myMatchRepositoryOptions = matchRepositoryOptions;
            mySteamUserRepositoryOptions = steamUserRepositoryOptions;
            myLogger = logger;
        }

        public MatchMakingDemoCollector(DbContextOptions<MatchRepository> matchRepositoryOptions, DbContextOptions<SteamUserRepository> steamUserRepositoryOptions)
            : this(matchRepositoryOptions, steamUserRepositoryOptions, UnitTestLoggerCreator.CreateUnitTestLogger<MatchMakingDemoCollector>()) { }

        public async Task ProcessNewMatches()
        {
            using var matchRepository = new MatchRepository(myMatchRepositoryOptions);

            var newSharingCodes = await GetNewSharingCodes();
            myLogger.LogInformation($"Retrieved {newSharingCodes.Count} new sharing codes: {string.Join(", ", newSharingCodes)}");
            var newMatches = new List<Match>();

            foreach (var sharingCode in newSharingCodes)
            {
                var gameRequest = ShareCode.Decode(sharingCode);
                myLogger.LogTrace($"Decoded sharing code {sharingCode} into Request with Match ID: {gameRequest.MatchId}, Outcome ID: {gameRequest.OutcomeId} and Token: {gameRequest.Token}");
                var demo = new Demo { ShareCode = sharingCode, GameRequest = gameRequest };
                Match match;

                var gameCoordinatorClient = new GameCoordinatorClient();
                try
                {
                    match = gameCoordinatorClient.GetMatchInfo(demo);
                }
                catch (Exception)
                {
                    myLogger.LogDebug($"Couldn't get download url for sharing code {sharingCode}. See previous logs/exceptions for explanation. Continuing.");
                    continue;
                }

                string demoFilePath;
                try
                {
                    demoFilePath = await DemoDownloader.DownloadAndDecompressDemo(match.Demo.DownloadURL);
                }
                catch (DemoDownloaderException exception)
                {
                    myLogger.LogDebug($"Demo downloading or decompressing failed: {exception.Message}");
                    continue;
                }

                myLogger.LogTrace($"Downloaded and decompressed demo file for sharing code {sharingCode}. Analyzing now.");
                match.Demo.FilePath = demoFilePath;

                var demoReader = new DemoReader(match);
                demoReader.Read();
                newMatches.Add(demoReader.Match);
            }

            myLogger.LogInformation($"Downloaded and analyzed {newMatches.Count} new matches (from {newSharingCodes.Count} new sharing codes). Adding to database...");
            matchRepository.Matches.AddRange(newMatches);
            matchRepository.SaveChanges();
        }

        private async Task<List<string>> GetNewSharingCodes()
        {
            using var userRepository = new SteamUserRepository(mySteamUserRepositoryOptions);
            using var matchRepository = new MatchRepository(myMatchRepositoryOptions);

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
