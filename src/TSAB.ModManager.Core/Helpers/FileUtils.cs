using System;
using System.IO;

namespace TSAB.ModManager.Core.Helpers
{
    public static class FileUtils
    {
        /// <summary>
        /// Copies a file safely with an automatic backup if the target file already exists.
        /// </summary>
        public static void SafeCopy(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                string backupPath = destinationPath + ".bak";
                File.Copy(destinationPath, backupPath, true);
            }

            File.Copy(sourcePath, destinationPath, true);
        }

        /// <summary>
        /// Deletes a file safely, preventing exceptions if the file is in use.
        /// </summary>
        public static bool SafeDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Unable to delete file: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Reads the contents of a file safely and returns it as a string.
        /// </summary>
        public static string SafeReadFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    return File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Unable to read file: {ex.Message}");
            }
            return string.Empty;
        }
    }
}
