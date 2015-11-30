using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csgotvsux
{
    public enum CsGoEventType
    {
        unknown,
        player_killed,
        round_start,
        bomb_pickup,
        round_end,
        bomb_dropped,
        bomb_planted,
        bomb_exploded,
        bomb_beginplant,
        bomb_begindefuse,
        bomb_defused,
        cs_pre_restart,
        round_freeze_end,
        announce_phase_end,
    }
}
