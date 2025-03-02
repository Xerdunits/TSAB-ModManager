using System;
using System.Diagnostics;
using System.IO;

namespace TSAB.ModManager.Core.Helpers
{
    public static class LuaValidator
    {
        private static string GetLuacPath()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string luacPath = Path.Combine(basePath, "TSAB.ModManager.Tools", "luac.exe");

            if (!File.Exists(luacPath))
                throw new FileNotFoundException($"[ERROR] luac.exe not found at {luacPath}");

            return luacPath;
        }

        public static bool ValidateLuaSyntax(string luaFilePath)
        {
            if (!File.Exists(luaFilePath))
            {
                Console.WriteLine($"[ERROR] LUA file not found: {luaFilePath}");
                return false;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = GetLuacPath(),
                Arguments = $"-p \"{luaFilePath}\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string errors = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(errors))
                {
                    Console.WriteLine($"[ERROR] LUA Syntax error in {luaFilePath}:\n{errors}");
                    return false;
                }
            }

            Console.WriteLine($"[OK] LUA syntax is valid: {luaFilePath}");
            return true;
        }
    }
}