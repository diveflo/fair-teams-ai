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
    public sealed class RankUpdater : IHostedService
    {
        private readonly IServiceScopeFactory myScopeFactory;
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<RankUpdater> myLogger;
        private const int myRankCheckerTriggerInMinutes = 360;
        private const int myRankCheckerTriggerOffsetInMinutes = 15;
        private Timer myRankCheckerSchedule;

        public RankUpdater(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            myScopeFactory = scopeFactory;
            myLoggerFactory = loggerFactory;
            myLogger = loggerFactory.CreateLogger<RankUpdater>();
        }

        public RankUpdater(IServiceScopeFactory scopeFactory) : this(scopeFactory, UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation($"MatchMakingDemoCollector timed hosted service started. {System.Environment.NewLine}" +
                $"Rank checker schedule: {myRankCheckerTriggerOffsetInMinutes} min offset, trigger every {myRankCheckerTriggerInMinutes}");

            myRankCheckerSchedule = new Timer(CheckForRankChanges, null, TimeSpan.FromMinutes(myRankCheckerTriggerOffsetInMinutes), TimeSpan.FromMinutes(myRankCheckerTriggerInMinutes));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("MatchMakingDemoCollector timed hosted service is stopping");
            myRankCheckerSchedule?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void CheckForRankChanges(object state)
        {
            var gameCoordinatorClient = new GameCoordinatorClient(myLoggerFactory);

            try
            {
                gameCoordinatorClient.ConnectAndLogin();
                UpdateRanksForPlayers(gameCoordinatorClient);
            }
            catch (GameCoordinatorException e)
            {
                myLogger.LogWarning($"Exception while trying to update ranks: {e.Message}");
            }
            finally
            {
                gameCoordinatorClient.Dispose();
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
