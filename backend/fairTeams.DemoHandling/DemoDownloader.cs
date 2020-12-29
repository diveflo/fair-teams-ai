using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace fairTeams.DemoHandling
{
    public class DemoDownloader
    {
        public static string DownloadDemoArchive(string downloadUrl)
        {
            var realFileExtension = GetRealFileExtensionFromBZip2DownloadURL(downloadUrl);

            var downloadLocation = Path.GetTempFileName().Replace(".tmp", realFileExtension + ".bz2");
            using var webClient = new WebClient();
            webClient.DownloadFile(downloadUrl, downloadLocation);
            return downloadLocation;
        }

        public static async Task<string> DecompressDemoArchive(string bz2FilePath)
        {
            if (!File.Exists(bz2FilePath))
            {
                throw new ArgumentException($"The archive file {bz2FilePath} does not exist.");
            }

            var taskCompletitionSource = new TaskCompletionSource<string>();
            string destination = bz2FilePath.Replace(".bz2", "");

            return await Task.Run(() =>
            {
                 BZip2.Decompress(File.OpenRead(bz2FilePath), File.Create(destination), true);
                return destination;
            });
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
