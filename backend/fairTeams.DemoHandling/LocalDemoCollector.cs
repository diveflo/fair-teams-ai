﻿using fairTeams.Core;
using fairTeams.DemoAnalyzer;
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
    public sealed class LocalDemoCollector : IHostedService
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
            myLogger.LogInformation($"LocalDemoCollector timed hosted service started (trigger interval: {myEveryMinutesToTriggerProcessing} minutes)");
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
            var blacklistedMatches = new List<Match>();
            var newDemoFiles = Directory.EnumerateFiles(myDemoWatchFolder).Where(x => x.EndsWith(".dem"));
            myLogger.LogDebug($"Found {newDemoFiles.Count()} new demo files in the watch folder");

            var demoBackuper = new DemoBackuper(myLoggerFactory.CreateLogger<DemoBackuper>());

            foreach (var demoFile in newDemoFiles)
            {
                var match = new Match { Demo = new Demo { FilePath = demoFile }, Date = File.GetCreationTime(demoFile) };

                try
                {
                    using var demoReader = new DemoReader(match);
                    demoReader.ReadHeader();
                    demoReader.Read();
                    match = demoReader.Match;
                    myLogger.LogTrace($"Finished analyzing demo file {Path.GetFileName(demoFile)}");
                    newMatches.Add(match);
                }
                catch (DemoReaderException e)
                {
                    match.Id += Guid.NewGuid().ToString();
                    match.Demo.Id = match.Id;
                    myLogger.LogWarning($"Analyzing demo from watch folder ({Path.GetFileName(demoFile)}) failed: {e.Message}. Adding to repository (id: {match.Id}) without stats -> blacklisted.");
                    match.PlayerResults.Clear();
                    match.CTScore = -1;
                    match.TScore = -1;
                    blacklistedMatches.Add(match);
                }

                BackupDemo(match.Demo, demoBackuper);
            }

            using var scope = myScopeFactory.CreateScope();
            myLogger.LogTrace($"Getting match repository to add {newMatches.Count} new matches and blacklist {blacklistedMatches.Count} demos.");
            var matchRepository = scope.ServiceProvider.GetRequiredService<MatchRepository>();
            matchRepository.AddMatchesAndSave(newMatches);
            matchRepository.AddMatchesAndSave(blacklistedMatches);
        }

        private void BackupDemo(Demo demo, DemoBackuper backuper)
        {
            try
            {
                backuper.BackupDemoFile(demo, true);
            }
            catch (Exception)
            {
                myLogger.LogWarning($"Backing up the downloaded demo file ({demo.FilePath}) failed.");
            }
        }
    }
}
