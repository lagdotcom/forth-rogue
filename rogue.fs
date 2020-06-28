include vid.fs
include map.fs
include entity.fs

27 constant k-esc

variable haltgame

create player
    <A White >FG A>
    rows 2 /
    cols 2 /
    '@'
new-entity

create npc
    <A Yellow >FG A>
    rows 2 /
    cols 2 / 5 -
    '@'
new-entity

: move-player ( mx my -- )
    over over                   ( mx my mx my )
    player entity-y @ +         ( mx my mx y )
    player entity-x @ under+    ( mx my x y )
    map-passable
    \ 0 32 at-xy .s
    if
        player clear-entity
        player move-entity
    else
        2drop
    then
;

: process-input ( -- )
    ekey
    case
        k-esc   of haltgame on endof
        'q'     of haltgame on endof

        k-up    of  0 -1 move-player endof
        k-right of  1  0 move-player endof
        k-down  of  0  1 move-player endof
        k-left  of -1  0 move-player endof
    endcase
;

: mainloop ( -- )
    haltgame off
    begin
        render-map
        draw-all-entities
        0 0 at-xy
        process-input
    haltgame @ until
;

page
npc add-entity
player add-entity

mainloop
0 attr!
bye
