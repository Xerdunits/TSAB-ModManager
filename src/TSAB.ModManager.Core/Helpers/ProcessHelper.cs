using System.Diagnostics;

namespace TSAB.ModManager.Core.Helpers
{
    public static class ProcessHelper
    {
        /// <summary>
        /// Checks if the Troubleshooter game is currently running.
        /// </summary>
        public static bool IsGameRunning()
        {
            return Process.GetProcessesByName("Troubleshooter_lsv").Length > 0;
        }

        /// <summary>
        /// Attempts to close the game process.
        /// </summary>
        public static bool CloseGame()
        {
            var processes = Process.GetProcessesByName("Troubleshooter_lsv");
            if (processes.Length == 0) return false;

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
