using System;
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
        /// <summary>
        ///     Steam AppId that identifies as Grim Dawn.
        /// </summary>
        public const int GrimDawnAppId = 219990;

        private static DirectoryInfo installDirectory;
        private static FileInfo grimDawnExe;
        private static DirectoryInfo resourceDirectory;

        /// <summary>
        ///     Sets the install directory to find Grim Dawn's files in.
        /// </summary>
        /// <param name="path">Path that points to a directory that contains the Grim Dawn's executable.</param>
        /// <returns>True if the path verifies as the Grim Dawn installation directory.</returns>
        public static bool SetInstallDirectory(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return false;

            return File.Exists(Path.Combine(path, "Grim Dawn.exe"));
        }

        /// <summary>
        ///     Searches for Grim Dawn installation directory or returns it if it was found previously.
        /// </summary>
        /// <returns>Path to the installation directory of Grim Dawn or null if not found.</returns>
        public static async Task<DirectoryInfo> GetInstallDirectoryAsync()
        {
            if (installDirectory != null) return installDirectory;

            installDirectory = await Task.Run(() =>
            {
                // Steam
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                var uninstallKey =
                    baseKey.OpenSubKey(
                        $@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App {GrimDawnAppId}");
                return uninstallKey != null ? new DirectoryInfo((string)uninstallKey.GetValue("InstallLocation")) : null;

                // TODO: Check for non-steam installation if above check failed
            });

            return installDirectory;
        }

        /// <summary>
        ///     Searches for the Grim Dawn installation directory if it wasn't searched before and tries to find the 'Grim
        ///     Dawn.exe' file inside it.
        /// </summary>
        /// <returns>Full path to the Grim Dawn executable if found. Otherwise null.</returns>
        public static async Task<FileInfo> GetExeAsync()
        {
            if (grimDawnExe != null) return grimDawnExe;

            grimDawnExe = await Task.Run(async () =>
            {
                var dir = await GetInstallDirectoryAsync();
                var exePath = new FileInfo(Path.Combine(dir.FullName, "Grim Dawn.exe"));
                return exePath.Exists ? exePath : null;
            });
            return grimDawnExe;
        }

        /// <summary>
        ///     Searches for the Grim Dawn installation directory if it wasn't searched before and then finds the resources folder
        ///     inside it.
        /// </summary>
        /// <returns>Path to the resources folder of Grim Dawn.</returns>
        public static async Task<DirectoryInfo> GetResourcesDirectoryAsync()
        {
            if (resourceDirectory != null) return resourceDirectory;

            resourceDirectory = await Task.Run(async () =>
            {
                var dir = await GetInstallDirectoryAsync();
                var resourcePath = new DirectoryInfo(Path.Combine(dir.FullName, "resources"));
                return resourcePath.Exists ? resourcePath : null;
            });

            return resourceDirectory;
        }
    }
}