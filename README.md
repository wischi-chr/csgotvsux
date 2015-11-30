# CS:GO Demo Replay Controls

Install
=======
copy csgotvsux.exe into your csgo folder<br>
<sub><sup>_(SteamLibrary/steamapps/common/Counter-Strike Global Offensive/csgo/csgotvsux.exe)_</sup></sub>


Usage
=====
1. start csgotvsux.exe
2. start csgo demo
3. open csgo console
4. type "exec playback" [RETURN]


Bindings
========
(do work without running csgotvsux process)


Key        | Action
---------- | --------------------------
RETURN     | pause/resume
BACKSPACE  | -1000 ticks (~ 15s)
PAGE_UP    | faster
PAGE_DOWN  | slower
HOME       | restart match
END        | reset speed (timescale 1)


(only works with active csgotvsux.exe)


Key        | Action
---------- | -----------------------------------
NUMPAD_DEL |	start round select (beep sound)
NUMPAD 0-9 |	select round number (two digits)


Examples
--------
jump to warmup `[NUMPAD_DEL][NUMPAD_0][NUMPAD_0]`

jump to round 12 `[NUMPAD_DEL][NUMPAD_1][NUMPAD_2]`

jump to round 3 `[NUMPAD_DEL][NUMPAD_0][NUMPAD_3]`
