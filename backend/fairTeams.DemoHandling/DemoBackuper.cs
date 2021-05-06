using fairTeams.Core;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace fairTeams.DemoHandling
{
    public sealed class DemoBackuper : IDisposable
    {
        private readonly ILogger<DemoBackuper> myLogger;
        private readonly FtpClient myFtpClient;

        public DemoBackuper(ILogger<DemoBackuper> logger)
        {
            myLogger = logger;

            var ftpCredentials = new NetworkCredential(Settings.DemoBackupFTPUsername, Settings.DemoBackupFTPPassword);
            myFtpClient = new FtpClient(Settings.DemoBackupFTP, 1818, ftpCredentials);
            myFtpClient.AutoConnect();
        }

        public DemoBackuper() : this(UnitTestLoggerCreator.CreateUnitTestLogger<DemoBackuper>()) { }

        public void BackupDemoFile(Demo demo)
        {
            BackupDemoFile(demo, false);
        }

        public void BackupDemoFile(Demo demo, bool deleteFileAfterUpload)
        {
            var filePath = demo.FilePath;

            myLogger.LogInformation($"Trying to back up demo file ({filePath})");

            if (!System.IO.File.Exists(filePath))
            {
                myLogger.LogWarning($"The file to be backed up does not exist: {filePath}");
                throw new Exception($"The file to be backed up does not exist: {filePath}");
            }

            if (!myFtpClient.IsConnected)
            {
                myFtpClient.AutoConnect();
            }

            var remoteDemoFileName = demo.State == DemoState.ParseFailure ? $"{demo.Id}_parseFailure.dem" : $"{demo.Id}.dem";
            var remoteFilePath = $"/csgo/{remoteDemoFileName}";

            var uploadResultStatus = myFtpClient.UploadFile(filePath, remoteFilePath, FtpRemoteExists.Skip, false, FtpVerify.Retry);
            if (uploadResultStatus == FtpStatus.Failed)
            {
                myLogger.LogWarning($"Uploading the demo file ({remoteDemoFileName}) to the FTP failed.");
                throw new Exception($"Uploading the demo file ({remoteDemoFileName}) to the FTP failed.");
            }

            myLogger.LogInformation($"Successfully backed up demo file {remoteDemoFileName}");

            if (deleteFileAfterUpload)
            {
                myLogger.LogInformation($"Deleting backed up demo file {remoteDemoFileName}");
                System.IO.File.Delete(filePath);
            }
        }

        public void Dispose()
        {
            myFtpClient.Disconnect();
        }
    }
}
