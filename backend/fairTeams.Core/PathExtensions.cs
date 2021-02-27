using System;
using System.IO;
using System.Linq;

namespace fairTeams.Core
{
    public static class PathExtensions
    {
        public static string GetFileName(string path)
        {
            var isLinuxPath = IsLinuxPath(path);
            var systemIsLinux = IsLinux();

            // Path.GetFileName doesn't work for Windows paths running on a Linux system
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getfilename?view=net-5.0#System_IO_Path_GetFileName_System_String_
            if (!isLinuxPath && systemIsLinux)
            {
                var splitWindowsPath = path.Split("\\").ToList();
                return splitWindowsPath.Last();
            }

            return Path.GetFileName(path);
        }

        public static bool IsLinuxPath(string path)
        {
            return path.StartsWith("/");
        }

        public static bool IsLinux()
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }
    }
}
