using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class DemoDownloaderTests
    {
        [Fact]
        public void DownloadDemoArchive_SmallTestFile_SuccessfullyDownloaded()
        {
            var downloadUrl = @"https://fairteamsai.backend.files.entertainment720.eu/test.txt.bz2";
            var expectedHash = new byte[] { 148, 155, 249, 87, 238, 163, 193, 104, 89, 218, 236, 53, 69, 241, 46, 123 };

            var locallyDownloadedDemoFile = DemoDownloader.DownloadDemoArchive(downloadUrl);

            using var stream = File.OpenRead(locallyDownloadedDemoFile);
            var downloadedFileHash = MD5.Create().ComputeHash(stream);

            Assert.Equal(expectedHash, downloadedFileHash);
        }

        [Fact]
        public async Task DecompressDemoArchive_SmallTestFile_SuccessfullyDecompressedAsync()
        {
            var downloadUrl = @"https://fairteamsai.backend.files.entertainment720.eu/test.txt.bz2";
            var locallyDownloadedDemoFile = DemoDownloader.DownloadDemoArchive(downloadUrl);

            var decompressedFile = await DemoDownloader.DecompressDemoArchive(locallyDownloadedDemoFile);

            var content = File.ReadAllText(decompressedFile);
            Assert.Equal("hi", content);
        }
    }
}
