using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csgotvsux
{
    public class CsGoRound
    {
        public bool IsWarmup { get; private set; }
        public int StartTick { get; private set; }
        public int EndTick { get; private set; }
        public IEnumerable<CsGoEvent> Events { get; private set; }


        private CsGoRound(CsGoEvent[] Events)
        {
            this.Events = Events;
            StartTick = Events.Select(e => e.Tick).Min();
            EndTick = Events.Select(e => e.Tick).Max();
            IsWarmup = !Events.Where(e => e.Tick == StartTick && e.Type == CsGoEventType.bomb_pickup).Any();
        }

        public static IEnumerable<CsGoRound> FromEvents(IEnumerable<CsGoEvent> Events)
        {
            var events = Events.ToList();
            var start_ticks = new List<int>();

            foreach (var ev in events)
                if (ev.Type == CsGoEventType.round_start) start_ticks.Add(ev.Tick);
            if (!start_ticks.Contains(0)) start_ticks.Insert(0, 0);
            start_ticks.Sort();

            for (int i = 0; i < start_ticks.Count - 1; i++)
                yield return new CsGoRound(events.Where(e => e.Tick >= start_ticks[i] && e.Tick < start_ticks[i + 1]).OrderBy(e => e.Tick).ToArray());

            if (start_ticks.Count > 0)
                yield return new CsGoRound(events.Where(e => e.Tick >= start_ticks[start_ticks.Count - 1]).OrderBy(e => e.Tick).ToArray());

        }
    }
}
