using fairTeams.Core;
using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
            string archivedDemoFilePath = String.Empty;
            try
            {
                archivedDemoFilePath = DownloadDemoArchive(downloadUrl).Result;
            }
            catch (AggregateException exception)
            {
                var innerExceptions = exception.InnerExceptions;

                if (innerExceptions.Any(x => x is HttpRequestException))
                {
                    throw new DemoNotAvailableException();
                }
            }
            catch (HttpRequestException)
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

        public async Task<string> DownloadDemoArchive(string downloadUrl)
        {
            myLogger.LogTrace("Trying to download: {downloadUrl}", downloadUrl);
            var realFileExtension = GetRealFileExtensionFromBZip2DownloadURL(downloadUrl);
            var downloadLocation = Path.GetTempFileName().Replace(".tmp", realFileExtension + ".bz2");
            myLogger.LogTrace("Downloading to local temp file: {downloadLocation}", downloadLocation);
            var fileInfo = new FileInfo(downloadLocation);

            using var webClient = new HttpClient();
            var response = await webClient.GetAsync(downloadUrl);
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.Create(fileInfo.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);

            return fileInfo.FullName;
        }

        public static string DecompressDemoArchive(string bz2FilePath)
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
