using fairTeams.Core;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;

namespace fairTeams.DemoHandling
{
    public class DemoDownloader
    {
        private readonly ILogger<DemoDownloader> myLogger;

        public DemoDownloader(ILogger<DemoDownloader> logger)
        {
            myLogger = logger;
        }

        public DemoDownloader() : this(UnitTestLoggerCreator.CreateUnitTestLogger<DemoDownloader>()) { }

        public string DownloadAndDecompressDemo(string downloadUrl)
        {
            string archivedDemoFilePath;
            try
            {
                archivedDemoFilePath = DownloadDemoArchive(downloadUrl);
            }
            catch (WebException)
            {
                throw new DemoNotAvailableException();
            }

            string demoFilePath;
            try
            {
                demoFilePath = DecompressDemoArchive(archivedDemoFilePath);
            }
            catch (ArgumentException)
            {
                throw new DemoDownloaderException($"The downloaded demo archive file ({archivedDemoFilePath} does not exist.");
            }

            return demoFilePath;
        }

        public string DownloadDemoArchive(string downloadUrl)
        {
            myLogger.LogTrace($"Trying to download: {downloadUrl}");
            var realFileExtension = GetRealFileExtensionFromBZip2DownloadURL(downloadUrl);
            var downloadLocation = Path.GetTempFileName().Replace(".tmp", realFileExtension + ".bz2");
            myLogger.LogTrace($"Downloading to local temp file: {downloadLocation}");
            using var webClient = new WebClient();
            webClient.DownloadFile(downloadUrl, downloadLocation);
            return downloadLocation;
        }

        public string DecompressDemoArchive(string bz2FilePath)
        {
            if (!File.Exists(bz2FilePath))
            {
                throw new ArgumentException($"The archive file {bz2FilePath} does not exist.");
            }

            string destination = bz2FilePath.Replace(".bz2", "");

            BZip2.Decompress(File.OpenRead(bz2FilePath), File.Create(destination), true);

            return destination;
        }

        private static string GetRealFileExtensionFromBZip2DownloadURL(string url)
        {
            var urlWithoutBZip2Extension = url[0..^4];
            var indexOfRealFileExtension = urlWithoutBZip2Extension.LastIndexOf(".");
            var lengthOfRealFileExtension = urlWithoutBZip2Extension.Length - indexOfRealFileExtension;
            return urlWithoutBZip2Extension.Substring(indexOfRealFileExtension, lengthOfRealFileExtension);
        }
    }
}
