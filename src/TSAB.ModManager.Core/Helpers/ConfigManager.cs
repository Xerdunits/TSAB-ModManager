using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TSAB.ModManager.Core.Helpers
{
    public static class ConfigManager
    {
        private static string GetConfigPath()
        {
            string configFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            return configFolder;
        }

        public static void SaveHashList(Dictionary<string, string> hashList)
        {
            string hashFilePath = Path.Combine(GetConfigPath(), "hashlist.json");

            if (hashList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[WARNING] Attempted to save an empty hashlist! Saving an empty file with a warning.");
            }

            // Add a warning message at the beginning of the JSON file
            string jsonContent = JsonConvert.SerializeObject(hashList, Formatting.Indented);
            jsonContent = "// Do not manually modify, edit or delete this file!\n" + jsonContent;

            File.WriteAllText(hashFilePath, jsonContent);
            System.Diagnostics.Debug.WriteLine($"[INFO] Hashlist saved with {hashList.Count} entries.");
        }

        public static void UpdateHashList(Dictionary<string, string> newHashList)
        {
            if (newHashList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] newHashList is EMPTY before saving! Skipping update.");
                return; // Prevent saving an empty list
            }

            Dictionary<string, string> currentHashes = LoadHashList();

            // Merge new hashes into existing list
            foreach (var entry in newHashList)
            {
                currentHashes[entry.Key] = entry.Value;
            }

            SaveHashList(currentHashes);
            System.Diagnostics.Debug.WriteLine($"[INFO] Hash list updated. Total files tracked: {currentHashes.Count}");
        }

        public static Dictionary<string, string> LoadHashList()
        {
            string hashFilePath = Path.Combine(GetConfigPath(), "hashlist.json");

            if (!File.Exists(hashFilePath))
            {
                System.Diagnostics.Debug.WriteLine("[INFO] Hashlist not found, returning empty list.");
                return new Dictionary<string, string>(); // Return an empty list, but do NOT delete the file
            }

            try
            {
                string jsonContent = File.ReadAllText(hashFilePath);

                // Check if the file contains a warning comment and remove it before parsing
                if (jsonContent.StartsWith("//"))
                {
                    int firstNewline = jsonContent.IndexOf('\n');
                    if (firstNewline != -1)
                    {
                        jsonContent = jsonContent.Substring(firstNewline + 1);
                    }
                }

                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load hashlist.json: {ex.Message}");
                return new Dictionary<string, string>(); // Fail-safe: Return empty list, do NOT crash
            }
        }
    }
}