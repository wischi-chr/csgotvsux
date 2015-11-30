using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace process_important_ticks
{
    class Program
    {
        static readonly string spacer = "============================================================";

        static void Main(string[] args)
        {
            //Get current working Dir
            var path = Directory.GetCurrentDirectory();

            //Override local csgo location
            OverrideDebugPath(ref path);

            if (!path.ToLower().EndsWith(@"\steamapps\common\Counter-Strike Global Offensive\csgo".ToLower()))
            {
                Console.WriteLine("Nicht im CS:GO Verzeichnis!");
                Console.ReadKey();
                return;
            }

            //string csgoPath = @"D:\SteamLibrary\steamapps\common\Counter-Strike Global Offensive\csgo\";
            string writeScript = Path.Combine(path, @"cfg\playback_game.cfg");
            string consoleLog = Path.Combine(path, "console.log");
            string lastHash = "";

            while (true)
            {
                if (lastHash != "") Thread.Sleep(2500);
                Console.Write(".");
                var g = CsGoGame.LastFromLogFile(consoleLog);
                if (g == null)
                {
                    lastHash = "null";
                    continue;
                }

                if (lastHash == g.GameHash) continue;
                lastHash = g.GameHash;
                Console.WriteLine();
                Console.WriteLine(spacer);
                Console.WriteLine("New Game: " + lastHash);
                g.WriteDemoNavScript(writeScript);
                Console.WriteLine(spacer);
                Console.WriteLine();
            }
        }

        [Conditional("DEBUG")]
        static void OverrideDebugPath(ref string path)
        {
            //override only if debugger is attached
            if (Debugger.IsAttached)
                path = @"D:\SteamLibrary\steamapps\common\Counter-Strike Global Offensive\csgo";
        }
    }
}
