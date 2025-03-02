using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace TSAB.ModManager.Core.Helpers
{
    public static class GamePathFinder
    {
        // Standard Steam paths where Troubleshooter might be installed
        private static readonly string[] possiblePaths =
        {
            @"C:\Program Files (x86)\Steam\steamapps\common\Troubleshooter",
            @"C:\Program Files\Steam\steamapps\common\Troubleshooter",
            @"D:\Steam\steamapps\common\Troubleshooter",
            @"E:\Steam\steamapps\common\Troubleshooter"
        };

        /// <summary>
        /// Checks standard installation paths.
        /// </summary>
        public static string FindGameInStandardPaths()
        {
            string foundPath = possiblePaths.FirstOrDefault(Directory.Exists);
            if (!string.IsNullOrEmpty(foundPath))
                Debug.WriteLine($"[INFO] Found game in standard path: {foundPath}");

            return foundPath;
        }

        /// <summary>
        /// Reads Steam's libraryfolders.vdf to find additional installation locations.
        /// </summary>
        public static string FindGameInSteamLibraries()
        {
            string steamConfigPath = @"C:\Program Files (x86)\Steam\steamapps\libraryfolders.vdf";

            if (!File.Exists(steamConfigPath))
            {
                Debug.WriteLine("[WARNING] Steam library file not found.");
                return null;
            }

            try
            {
                foreach (string line in File.ReadLines(steamConfigPath))
                {
                    // Extracts possible drive paths from the VDF
                    Match match = Regex.Match(line, "\"[0-9]+\"\\s*\"(.+?)\"");
                    if (match.Success)
                    {
                        string libraryPath = match.Groups[1].Value.Replace("\\\\", "\\");
                        string possibleGamePath = Path.Combine(libraryPath, "steamapps", "common", "Troubleshooter");

                        if (Directory.Exists(possibleGamePath))
                        {
                            Debug.WriteLine($"[INFO] Found game in Steam library: {possibleGamePath}");
                            return possibleGamePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Error reading Steam library paths: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Searches for Troubleshooter_lsv.exe in a limited set of known folders to improve performance.
        /// </summary>
        public static string FindGameExe(string driveLetter)
        {
            string[] knownGameFolders =
            {
                $"{driveLetter}:\\Games",
                $"{driveLetter}:\\Program Files",
                $"{driveLetter}:\\Program Files (x86)",
                $"{driveLetter}:\\Steam"
            };

            foreach (string basePath in knownGameFolders)
            {
                try
                {
                    if (Directory.Exists(basePath))
                    {
                        string[] files = Directory.GetFiles(basePath, "Troubleshooter_lsv.exe", SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            Debug.WriteLine($"[INFO] Found game executable at: {files[0]}");
                            return files[0];
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine($"[WARNING] Access denied to: {basePath}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] Error searching for game in {basePath}: {ex.Message}");
                }
            }

            Debug.WriteLine("[WARNING] Game executable not found.");
            return null;
        }

        /// <summary>
        /// Attempts to detect the game installation path.
        /// </summary>
        public static string DetectGamePath()
        {
            // Step 1: Check standard paths
            string foundPath = FindGameInStandardPaths();
            if (!string.IsNullOrEmpty(foundPath))
                return foundPath;

            // Step 2: Check Steam library paths
            foundPath = FindGameInSteamLibraries();
            if (!string.IsNullOrEmpty(foundPath))
                return foundPath;

            // Step 3: Search for Troubleshooter_lsv.exe in common drives
            string[] drives = { "C", "D", "E" };
            foreach (var drive in drives)
            {
                foundPath = FindGameExe(drive);
                if (!string.IsNullOrEmpty(foundPath))
                    return Path.GetDirectoryName(foundPath);
            }

            Debug.WriteLine("[ERROR] Game not found.");
            return null;
        }
    }
}
