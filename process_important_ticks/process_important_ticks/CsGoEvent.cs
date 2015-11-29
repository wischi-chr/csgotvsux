using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace process_important_ticks
{
    public class CsGoEvent
    {
        //Wrapper Properties
        public int Tick { get { return tick; } }
        public string Comment { get { return comment; } }
        public CsGoEventType Type { get { return type; } }
        public string Killer { get { if (type != CsGoEventType.player_killed) throw new NotSupportedException(); return p1; } }
        public string Victim { get { if (type != CsGoEventType.player_killed) throw new NotSupportedException(); return p2; } }
        public string Weapon { get { if (type != CsGoEventType.player_killed) throw new NotSupportedException(); return weapon; } }
        public bool WasHeadshot { get { if (type != CsGoEventType.player_killed) throw new NotSupportedException(); return headshot; } }

        //generic event fields
        private readonly CsGoEventType type;
        private readonly int tick;
        private readonly string comment;

        //player_killed event fields
        private readonly string p1;
        private readonly string p2;
        private readonly string weapon;
        private readonly bool headshot;

        //cache event type mapping
        private static readonly Dictionary<string, CsGoEventType> eventMapping = new Dictionary<string, CsGoEventType>();

        static CsGoEvent()
        {
            //build event type (enum) mapping
            foreach (CsGoEventType v in Enum.GetValues(typeof(CsGoEventType)))
                eventMapping.Add(v.ToString(), v);
        }

        public static CsGoEvent Parse(string EventRow)
        {
            var m = Regex.Match(EventRow, "Tick: ([0-9]+) (.*)");
            if (!m.Success)
            {
                Debug.WriteLine("Unparseable Event: " + EventRow);
                return null;
            }

            var t = int.Parse(m.Groups[1].Value.Trim());
            var c = m.Groups[2].Value.Trim();
            var y = CsGoEventType.player_killed;

            if (c.StartsWith("Event: "))
            {
                c = c.Substring(7);
                if (eventMapping.ContainsKey(c))
                    y = eventMapping[c];
                else
                    y = CsGoEventType.unknown;
            }

            return new CsGoEvent(t, y, c);
        }

        protected CsGoEvent(int Tick, CsGoEventType Type, string Comment)
        {
            tick = Tick;
            type = Type;
            comment = Comment;

            if (type == CsGoEventType.player_killed)
            {
                if (comment.StartsWith(": ")) comment = comment.Substring(2);
                var m = Regex.Match(comment, "(.*) killed (.*)( with a headshot)? using a (.*)");
                if (!m.Success)
                {
                    type = CsGoEventType.unknown;
                }
                else
                {
                    p1 = m.Groups[1].Value;
                    p2 = m.Groups[2].Value;
                    headshot = m.Groups.Count == 5;
                    weapon = m.Groups[headshot ? 4 : 3].Value;
                }
            }
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
