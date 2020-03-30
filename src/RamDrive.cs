using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RamDisk
{
    /// <summary>
    /// RamDrive
    /// </summary>
    public static class RamDrive
    {
        private static readonly string filePath;

        /// <summary>
        /// Load imdisk.exe in temp folder
        /// </summary>
        static RamDrive()
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

        /// <summary>
        /// Mount a drive on system memory
        /// </summary>
        /// <param name="megaBytes">Size in mega bytes</param>
        /// <param name="fileSystem">File system format</param>
        /// <param name="driveLetter">Drive letter</param>
        /// <param name="volumeLabel">Volume name</param>
        /// <returns>Returns output of imdisk console</returns>
        /// <remarks>
        /// https://gist.github.com/stokito/19e377c872dd85ee4445eabce97fa2e8
        /// </remarks>
        public static string Mount(int megaBytes, FileSystem fileSystem = FileSystem.NTFS, char driveLetter = 'Z', string volumeLabel = "RamDisk")
        {
            if (megaBytes <= 0)
                throw new ArgumentException("Allocation size must be greater than zero.", nameof(megaBytes));
            if (string.IsNullOrWhiteSpace(volumeLabel))
                throw new ArgumentNullException("Volume label muste be not null or empty.", nameof(volumeLabel));
            if (DriveInfo.GetDrives().Any(d => d.Name.ToUpper()[0] == driveLetter))
                throw new InvalidOperationException($"Drive '{driveLetter}' already exists.");

            var processStart = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = filePath,
                Arguments = $"-a -s {megaBytes}M -m {driveLetter}: -p \"/fs:{fileSystem} /q /v:{volumeLabel} /y\"",
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

        /// <summary>
        /// Unmount drive by it's letter name
        /// </summary>
        /// <param name="driveLetter">Drive letter to unmount</param>
        /// <returns>Returns output of imdisk console</returns>
        public static string Unmount(char driveLetter = 'Z')
        {
            if (DriveInfo.GetDrives().Any(d => d.Name.ToUpper()[0] == driveLetter) == false)
                throw new InvalidOperationException($"Drive '{driveLetter}' does not exists.");

            var processStart = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = filePath,
                Arguments = $"-D -m {driveLetter}:",
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

        /// <summary>
        /// Load imdisk.exe in temp folder
        /// </summary>
        /// <returns>Returns path of imdisk.exe in temp folder</returns>
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
