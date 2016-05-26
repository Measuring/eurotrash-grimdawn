using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Eurotrash.GrimDawn.Core.Discovery.Game
{
    /// <summary>
    ///     Utility class for accessing the game files of Grim Dawn.
    /// </summary>
    public static class GameFiles
    {
        public const int GrimDawnAppId = 219990;

        private static string installDirectory;
        private static FileInfo grimDawnExe;

        /// <summary>
        ///     Searches for Grim Dawn installation directory or returns it if it was found previously.
        /// </summary>
        public static async Task<string> GetInstallDirectoryAsync()
        {
            if (!string.IsNullOrEmpty(installDirectory)) return installDirectory;

            installDirectory = (string)await Task.Run(() =>
            {
                // Steam
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                var uninstallKey = baseKey.OpenSubKey($@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App {GrimDawnAppId}");
                return uninstallKey?.GetValue("InstallLocation");
            });

            return installDirectory;
        }

        public static async Task<FileInfo> GetGrimDawnExeAsync()
        {
            if (grimDawnExe != null) return grimDawnExe;

            grimDawnExe = await Task.Run(async () =>
            {
                var dir = await GetInstallDirectoryAsync();
                var exePath = new FileInfo(Path.Combine(dir, "Grim Dawn.exe"));
                return exePath.Exists ? exePath : null;
            });
            return grimDawnExe;
        }
    }
}