using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace TSAB.ModManager.Core.Helpers
{
    public static class VerificationManager
    {
        private static bool debugMode = false; // Set to false for full verification

        private static string GetDataPath()
        {
            string gamePath = GamePathFinder.DetectGamePath();
            if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Game path not found! Cannot verify files.");
                return null;
            }

            string dataPath = Path.Combine(gamePath, "Data");

            if (!Directory.Exists(dataPath))
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Data folder not found: {dataPath}");
                return null;
            }

            return dataPath;
        }
        public static List<string> GetModifiedFiles()
        {
            string dataPath = GetDataPath();
            if (string.IsNullOrEmpty(dataPath))
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Data path is invalid.");
                return new List<string>();
            }

            string hashFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "hashlist.json");
            if (!File.Exists(hashFilePath))
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Hashlist file not found!");
                return new List<string>();
            }

            Dictionary<string, string> originalHashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(hashFilePath));
            List<string> modifiedFiles = new List<string>();

            // Get all files in the Data folder
            var allFiles = Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories);

            // Apply debug mode limit
            if (debugMode)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Running in debug mode. Limiting file check to 10 files.");
                allFiles = allFiles.Take(10).ToArray();
            }

            foreach (var file in allFiles)
            {
                string currentHash = ComputeMD5(file);

                if (!originalHashes.TryGetValue(file, out string storedHash) || storedHash != currentHash)
                {
                    modifiedFiles.Add(file); // Either new or modified file
                }
            }

            return modifiedFiles;
        }
        public static string ComputeMD5(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}