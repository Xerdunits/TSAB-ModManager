using System;
using System.IO;

namespace TSAB.ModManager.Core.Helpers
{
    public static class BackupManager
    {
        public static void BackupOriginalFile(string filePath)
        {
            string backupPath = filePath + ".backup";

            if (!File.Exists(backupPath))
                File.Copy(filePath, backupPath, true);
        }

        public static void CreateVersionedBackup(string filePath)
        {
            string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            Directory.CreateDirectory(backupFolder);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFile = Path.Combine(backupFolder, $"{Path.GetFileName(filePath)}_{timestamp}.bak");

            File.Copy(filePath, backupFile, true);
        }
        public static bool RestoreFile(string filePath)
        {
            string backupPath = filePath + ".backup";

            if (File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, filePath, true);
                    System.Diagnostics.Debug.WriteLine($"[INFO] Restored backup for: {filePath}");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to restore {filePath}: {ex.Message}");
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[WARNING] No backup found for: {filePath}");
            return false;
        }
        public static bool RestoreLastBackup(string filePath)
        {
            string backupPath = filePath + ".backup";

            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath, true);
                return true;
            }
            return false;
        }

        public static void CleanupOldBackups(string fileName)
        {
            string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            if (!Directory.Exists(backupFolder)) return;

            var backups = Directory.GetFiles(backupFolder, $"{fileName}_*.bak")
                                   .OrderByDescending(File.GetCreationTime)
                                   .Skip(5) // Keep last 5 backups
                                   .ToList();

            foreach (var oldBackup in backups)
                File.Delete(oldBackup);
        }
    }
}