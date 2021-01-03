using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public class LocalDemoCollector : IHostedService
    {
        private readonly IServiceScopeFactory myScopeFactory;
        private readonly ILoggerFactory myLoggerFactory;
        private readonly ILogger<LocalDemoCollector> myLogger;
        private const int myEveryMinutesToTriggerProcessing = 30;
        private Timer myTimer;
        private readonly string myDemoWatchFolder;

        public LocalDemoCollector(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
        {
            myScopeFactory = scopeFactory;
            myLoggerFactory = loggerFactory;
            myLogger = loggerFactory.CreateLogger<LocalDemoCollector>();

            myDemoWatchFolder = Settings.DemoWatchFolder;
            if (!Directory.Exists(myDemoWatchFolder))
            {
                Directory.CreateDirectory(myDemoWatchFolder);
            }
        }

        public LocalDemoCollector(IServiceScopeFactory scopeFactory) : this(scopeFactory, UnitTestLoggerCreator.CreateUnitTestLoggerFactory()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("LocalDemoCollector timed hosted service started");
            myTimer = new Timer(ProcessNewMatches, null, TimeSpan.Zero, TimeSpan.FromMinutes(myEveryMinutesToTriggerProcessing));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("LocalDemoCollector timed hosted service is stopping");
            myTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void ProcessNewMatches(object state)
        {
            var newMatches = new List<Match>();
            var newDemoFiles = Directory.EnumerateFiles(myDemoWatchFolder).Where(x => x.EndsWith(".dem"));

            foreach (var demoFile in newDemoFiles)
            {
                var match = new Match { Demo = new Demo { FilePath = demoFile }, Date = File.GetCreationTime(demoFile) };

                try
                {
                    using var demoReader = new DemoReader(match);
                    demoReader.ReadHeader();
                    demoReader.Read();
                    match = demoReader.Match;
                }
                catch (DemoReaderException e)
                {
                    myLogger.LogWarning($"Analyzing demo from watch folder ({Path.GetFileName(demoFile)}) failed: {e.Message}");
                    continue;
                }

                myLogger.LogTrace($"Finished analyzing demo file {Path.GetFileName(demoFile)}");
                newMatches.Add(match);

                myLogger.LogTrace($"Deleting local demo file {Path.GetFileName(demoFile)}");
                File.Delete(demoFile);
            }

            if (newMatches.Any())
            {
                using var scope = myScopeFactory.CreateScope();
                myLogger.LogInformation($"Getting match repository to save {newMatches.Count} potentially new matches.");
                var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();

                foreach (var match in newMatches)
                {
                    try
                    {
                        matchRepository.Matches.Add(match);
                        matchRepository.SaveChanges();
                    }
                    catch (DbUpdateException e)
                    {
                        var innerSqlException = e.InnerException as SqliteException;
                        var isAlreadyAdded = innerSqlException.SqliteErrorCode == 19;
                        if (isAlreadyAdded)
                        {
                            myLogger.LogDebug($"Match with id: {match.Id} already exists in repository.");
                            continue;
                        }

                        throw;
                    }
                }
            }
        }
    }
}
