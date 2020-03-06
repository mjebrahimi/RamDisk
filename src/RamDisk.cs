using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RamDisk
{
    public static class RamDisk
    {
        private static readonly string filePath;

        static RamDisk()
        {
            filePath = LoadImdiskExe();
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            };
        }

        //https://gist.github.com/stokito/19e377c872dd85ee4445eabce97fa2e8
        public static string Mount(int megaBytes, FileSystem fileSystem = FileSystem.NTFS, char driverLetter = 'Z', string volumeLabel = "RamDisk")
        {
            if (megaBytes <= 0)
                throw new ArgumentException("Allocation size must be greater than zero", nameof(megaBytes));
            if (string.IsNullOrWhiteSpace(volumeLabel))
                throw new ArgumentNullException("Volume label muste be not null or empty", nameof(volumeLabel));

            var processStart = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = filePath,
                Arguments = $"-a -s {megaBytes}M -m {driverLetter}: -p \"/fs:{fileSystem} /q /v:{volumeLabel} /y\"",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process
            {
                StartInfo = processStart
            };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(5000);

            return output;
        }

        public static string Unmount(char driverLetter = 'Z')
        {
            var processStart = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = filePath,
                Arguments = $"-D -m {driverLetter}:",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process
            {
                StartInfo = processStart
            };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(5000);

            return output;
        }

        private static string LoadImdiskExe()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var tempPath = Path.GetTempFileName();
            using (var stream = assembly.GetManifestResourceStream("RamDisk.imdisk.exe"))
            using (var file = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write))
                stream.CopyTo(file);
            return tempPath;
        }
    }
}
