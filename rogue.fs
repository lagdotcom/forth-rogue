include utils.fs
include vid.fs
include rect.fs
include entity.fs
include map.fs

27 constant k-esc

variable haltgame

create player
    <A White >FG A>
    0
    0
    '@'
new-entity

: move-player ( mx my -- )
    over over                   ( mx my mx my )
    player entity-y @ +         ( mx my mx y )
    player entity-x @ under+    ( mx my x y )
    map-passable
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
        draw-all-entities
        render-map
        present

        0 0 at-xy
        process-input
    haltgame @ until
;

vid-clear
player 6 10 30 generate-map
player add-entity

mainloop
0 attr!
bye
