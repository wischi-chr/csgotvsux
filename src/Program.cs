using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace csgotvsux
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

            //setup locations
            string mainScriptName = @"cfg\playback.cfg";
            string gameScriptName = @"cfg\playback_game.cfg";
            string consoleLog = Path.Combine(path, "console.log");
            string mainScript = Path.Combine(path, mainScriptName);
            string gameScript = Path.Combine(path, gameScriptName);

            //write playback.cfg
            using (var cfgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("csgotvsux.res.playback.cfg"))
                if (cfgStream != null)
                    using (var reader = new StreamReader(cfgStream))
                    {
                        Console.WriteLine("Write {0}", mainScriptName);
                        File.WriteAllText(mainScript, reader.ReadToEnd());
                    }


            string lastHash = "";

            while (true)
            {
                if (lastHash != "") Thread.Sleep(2500);
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
                Console.WriteLine("New Game found.");
                Console.WriteLine("Hash:   {0}", lastHash.Substring(0, 6));
                Console.WriteLine("Rounds: {0} (+{1} Warmup)", g.NumOfRoundsDrill, g.NumOfRoundsWarmup);
                g.WriteDemoNavScript(gameScript);
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
