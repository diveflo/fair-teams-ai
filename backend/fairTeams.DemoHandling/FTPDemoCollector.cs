﻿using ByteSizeLib;
using fairTeams.Core;
using FluentFTP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public sealed class FTPDemoCollector : IHostedService
    {
        private readonly IServiceProvider myServiceProvider;
        private readonly ILogger<FTPDemoCollector> myLogger;
        private Timer myTimer;
        private readonly FtpClient myFtpClient;

        public int TriggerScheduleInMinutes { get; set; }

        public FTPDemoCollector(IServiceProvider serviceProvider, ILogger<FTPDemoCollector> logger)
        {
            myServiceProvider = serviceProvider;
            myLogger = logger;
            TriggerScheduleInMinutes = 30;

            myFtpClient = new FtpClient(Settings.CSGOServerFTP)
            {
                Credentials = new NetworkCredential(Settings.CSGOServerFTPUsername, Settings.CSGOServerFTPPassword)
            };
        }

        public FTPDemoCollector(IServiceProvider serviceProvider) : this(serviceProvider, UnitTestLoggerCreator.CreateUnitTestLogger<FTPDemoCollector>()) { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation($"FTPDemoCollector timed hosted service started (trigger interval: {TriggerScheduleInMinutes} minutes)");
            myTimer = new Timer(ProcessNewMatches, null, TimeSpan.Zero, TimeSpan.FromMinutes(TriggerScheduleInMinutes));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            myLogger.LogInformation("FTPDemoCollector timed hosted service is stopping");
            myTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void ProcessNewMatches(object state)
        {
            myFtpClient.Connect();

            var allDemoFiles = ListDemoFilesOnFTPServer();
            allDemoFiles = GetDemoFilesForFinishedMatches(allDemoFiles);
            myLogger.LogTrace($"Found {allDemoFiles.Count} demo files on FTP server.");
            var newDemoFiles = GetNewDemoFiles(allDemoFiles);
            myLogger.LogTrace($"{newDemoFiles.Count} of the found {allDemoFiles.Count} demo files on the FTP server are new.");

            var downloadedFilesAndStatus = DownloadDemoFiles(newDemoFiles);
            FixFileExtensionForSuccessfulDownloads(downloadedFilesAndStatus);
        }

        public List<FtpListItem> ListDemoFilesOnFTPServer()
        {
            var minimumFileSize = ByteSize.FromMegaBytes(10.0);

            if (!myFtpClient.IsConnected)
            {
                myFtpClient.Connect();
            }

            var demoFiles = new List<FtpListItem>();

            try
            {
                var names = myFtpClient.GetNameListing();
                var files = myFtpClient.GetListing();

                demoFiles = files
                    .Where(x => x.Type == FtpObjectType.File)
                    .Where(x => x.FullName.EndsWith(".dem"))
                    .Where(x => x.Size >= minimumFileSize.Bytes)
                    .ToList();
            }
            catch (FtpCommandException e)
            {
                myLogger.LogWarning($"Error while trying to look for new demo files on CSGO ftp server: {e.Message}");
            }
            catch (Exception e)
            {
                myLogger.LogError($"Unexpected exception occured: {e.Message}");
            }

            return demoFiles;
        }

        public List<FtpListItem> GetNewDemoFiles(List<FtpListItem> allDemoFiles)
        {
            using var scope = myServiceProvider.CreateScope();
            var matchRepository = scope.ServiceProvider.GetService<MatchRepository>();

            var existingDemoFileNames = matchRepository.Matches.AsQueryable().Select(x => PathExtensions.GetFileName(x.Demo.FilePath)).ToList();
            existingDemoFileNames.AddRange(Directory.EnumerateFiles(Settings.DemoWatchFolder).Where(x => x.EndsWith(".dem")));
            return allDemoFiles.Where(x => !existingDemoFileNames.Contains(x.Name)).ToList();
        }

        public IEnumerable<(string, FtpStatus)> DownloadDemoFiles(List<FtpListItem> newDemoFiles)
        {
            if (!myFtpClient.IsConnected)
            {
                myFtpClient.Connect();
            }

            var localPathsWithStatus = new List<(string, FtpStatus)>();

            foreach (var demoFile in newDemoFiles)
            {
                var downloadPathWithTmpExtension = Path.Combine(Settings.DemoWatchFolder, demoFile.Name) + ".tmp";
                var status = myFtpClient.DownloadFile(downloadPathWithTmpExtension, demoFile.FullName);
                File.SetCreationTime(downloadPathWithTmpExtension, demoFile.Modified);

                yield return (downloadPathWithTmpExtension, status);
            }
        }

        public void FixFileExtensionForSuccessfulDownloads(IEnumerable<(string, FtpStatus)> downloadedDemos)
        {
            foreach (var filePathAndStatus in downloadedDemos)
            {
                var path = filePathAndStatus.Item1;
                var status = filePathAndStatus.Item2;

                if (status == FtpStatus.Success)
                {
                    var fixedFilePath = path[0..^4];

                    try
                    {
                        File.Move(path, fixedFilePath);
                    }
                    catch (IOException e)
                    {
                        myLogger.LogWarning($"Error while attempting to rename {Path.GetFileName(path)}: {e.Message}");
                    }
                    finally
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        private static List<FtpListItem> GetDemoFilesForFinishedMatches(List<FtpListItem> files)
        {
            var finishedDemoFiles = new List<FtpListItem>();
            foreach (var file in files)
            {
                var tenMinutesAgo = DateTime.UtcNow - TimeSpan.FromMinutes(10.0);
                if (file.Modified <= tenMinutesAgo)
                {
                    finishedDemoFiles.Add(file);
                }
            }

            return finishedDemoFiles;
        }
    }
}
