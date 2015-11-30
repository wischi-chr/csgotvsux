using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace csgotvsux
{
    public class CsGoGame
    {
        public string GameHash { get { return game_hash; } }

        public int NumOfRounds { get { return num_rounds_drill + num_rounds_warmup; } }
        public int NumOfRoundsDrill { get { return num_rounds_drill; } }
        public int NumOfRoundsWarmup { get { return num_rounds_warmup; } }

        private static readonly string beginTag = "==BEGIN=BEGIN==[IMPORTANT_TICKS]==BEGIN=BEGIN==";
        private static readonly string endTag = "==END=END=END==[IMPORTANT_TICKS]==END=END=END==";
        private readonly CsGoRound[] rounds;
        private readonly string game_hash;

        private readonly int num_rounds_warmup;
        private readonly int num_rounds_drill;

        public static CsGoGame LastFromLogFile(string Name)
        {
            bool foundEnd = false;
            bool foundStart = false;
            var lines = new List<string>();

            using (var fs = File.Open(Name, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var line in RevFile.ReadLines(fs))
                {
                    if (line == endTag) foundEnd = true;
                    if (foundEnd && line == beginTag) foundStart = true;
                    if (foundEnd) lines.Add(line);
                    if (foundStart) break;
                }
            }

            if (!foundStart || !foundEnd) return null;
            lines.Reverse();
            return new CsGoGame(lines.Skip(2).Take(lines.Count - 3).ToArray());
        }

        private CsGoGame(string[] Events)
        {
            using (var sha = new SHA256Managed())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, Events)));
                game_hash = Convert.ToBase64String(hash);
            }

            var events = Events.Select(e => CsGoEvent.Parse(e)).ToArray();
            rounds = CsGoRound.FromEvents(events).ToArray();

            num_rounds_drill = 0;
            num_rounds_warmup = 0;
            foreach (var r in rounds)
            {
                if (r.IsWarmup) num_rounds_warmup++;
                else num_rounds_drill++;
            }
        }

        public void WriteDemoNavScript(string name)
        {
            var sb = new StringBuilder();
            int rnd = 0;
            sb.AppendLine("//alias roundticks");
            sb.AppendLine("alias \"jump_to_round_0\" \"demo_gototick 0\"");
            for (int i = 0; i < rounds.Length; i++)
            {
                if (rounds[i].IsWarmup) continue;
                rnd++;
                sb.AppendLine("alias \"jump_to_round_" + rnd + "\" \"demo_gototick " + rounds[i].StartTick + "\"");
            }
            rnd++;
            while (rnd <= 30)
                sb.AppendLine("alias \"jump_to_round_" + rnd++ + " \"beep_fail\"");

            sb.AppendLine();
            for (int i = 0; i < 30; i++)
            {
                sb.AppendLine("alias \"jump_to_round_" + (i + 1) + "_reset\" \"press_num_reset;jump_to_round_" + (i + 1) + ";\"");
            }
            sb.AppendLine();



            sb.Append("alias press_num_reset \"");
            for (int i = 0; i < 10; i++)
                sb.Append("alias press_num_" + i + " beep_fail;");
            sb.AppendLine("\"");


            sb.AppendLine("alias press_fail_reset \"beep_fail;press_num_reset;\"");
            sb.AppendLine("alias press_success_reset \"beep_success;press_num_reset;\"");

            sb.AppendLine();
            for (int i = 0; i < 4; i++)
            {
                sb.Append("alias set" + i + "0er \"");
                for (int j = 0; j < 10; j++)
                {
                    int rnum = (i * 10 + j);
                    if (rnum <= 30)
                    {
                        sb.Append("alias press_num_" + j + " jump_to_round_" + rnum + "_reset;");
                    }
                    else
                    {
                        sb.Append("alias press_num_" + j + " press_fail_reset;");
                    }
                }
                sb.AppendLine("\"");
            }
            sb.AppendLine();

            for (int i = 0; i < 10; i++)
            {
                if (i < 4)
                    sb.AppendLine("alias press_num_" + i + " set" + i + "0er");
                else
                    sb.AppendLine("alias press_num_" + i + " press_fail_reset;");
            }

            sb.AppendLine();

            sb.AppendLine("bind kp_ins press_num_0");
            sb.AppendLine("bind kp_end press_num_1");
            sb.AppendLine("bind kp_downarrow press_num_2");
            sb.AppendLine("bind kp_pgdn press_num_3");
            sb.AppendLine("bind kp_leftarrow press_num_4");
            sb.AppendLine("bind kp_5 press_num_5");
            sb.AppendLine("bind kp_rightarrow press_num_6");
            sb.AppendLine("bind kp_home press_num_7");
            sb.AppendLine("bind kp_uparrow press_num_8");
            sb.AppendLine("bind kp_pgup press_num_9");

            sb.AppendLine();
            sb.AppendLine("beep_success");
            var script = sb.ToString();
            File.WriteAllText(name, script);
        }
    }
}
