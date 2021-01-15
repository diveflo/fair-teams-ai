using fairTeams.Core;
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
    public sealed class ShareCodeCollector : IHostedService
    {
        private readonly IServiceScopeFactory myScopeFactory;
        private readonly ILogger<ShareCodeCollector> myLogger;
        private const int myEveryMinutesToTriggerProcessing = 10;
        private Timer myTimer;

        public ShareCodeCollector(IServiceScopeFactory scopeFactory, ILogger<ShareCodeCollector> logger)
        {
            myScopeFactory = scopeFactory;
            myLogger = logger;
        }

        public ShareCodeCollector(IServiceScopeFactory scopeFactory) : this(scopeFactory, UnitTestLoggerCreator.CreateUnitTestLogger<ShareCodeCollector>()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation($"MatchMakingDemoCollector timed hosted service started (trigger interval: {myEveryMinutesToTriggerProcessing} minutes)");
            myTimer = new Timer(CollectNewShareCodes, null, TimeSpan.Zero, TimeSpan.FromMinutes(myEveryMinutesToTriggerProcessing));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("ShareCodeCollector timed hosted service is stopping");
            myTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void CollectNewShareCodes(object state)
        {
            var shareCodes = new List<ShareCode>();

            foreach (var code in GetNewSharingCodes().Result)
            {
                shareCodes.Add(new ShareCode(code));
            }

            using var scope = myScopeFactory.CreateScope();
            var shareCodeRepository = scope.ServiceProvider.GetRequiredService<ShareCodeRepository>();
            shareCodeRepository.AddNew(shareCodes);
        }

        private async Task<List<string>> GetNewSharingCodes()
        {
            using var scope = myScopeFactory.CreateScope();
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();
            var userRepository = scope.ServiceProvider.GetRequiredService<SteamUserRepository>();
            var steamworksApi = scope.ServiceProvider.GetRequiredService<SteamworksApi>();

            var alreadyProcessedSharingCodes = matchRepository.Matches.Include("Demo").AsEnumerable().Select(x => x.Demo.ShareCode);
            var newSharingCodes = new List<string>();

            var steamUsers = userRepository.SteamUsers.ToList();

            foreach (var user in steamUsers)
            {
                var sharingCode = await steamworksApi.GetNextMatchSharingCode(user.SteamID.ToString(), user.AuthenticationCode, user.LastSharingCode);
                if (sharingCode.Equals("n/a") || sharingCode.Equals(user.LastSharingCode))
                {
                    myLogger.LogTrace($"No new sharing code for Steam ID: {user.SteamID}");
                    continue;
                }

                myLogger.LogTrace($"New sharing code {sharingCode} for Steam ID: {user.SteamID}");
                user.LastSharingCode = sharingCode;

                if (alreadyProcessedSharingCodes.Contains(sharingCode))
                {
                    myLogger.LogTrace($"Match for sharing code {sharingCode} already processed.");
                    continue;
                }

                if (newSharingCodes.Contains(sharingCode))
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
