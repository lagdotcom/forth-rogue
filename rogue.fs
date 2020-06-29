include defs.fs
include vars.fs
include utils.fs
include rect.fs
include entity.fs
include map.fs
include fov.fs
include vid.fs

: move-player ( mx my -- )
    over over                   ( mx my mx my )
    player entity-y @ +         ( mx my mx y )
    player entity-x @ under+    ( mx my x y )
    map-passable
    if
        player clear-entity
        player move-entity
        fov-recompute on
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
        fov-recompute if recompute-fov then
        render-map
        fov-recompute off
        draw-all-entities
        present

        player entity-x @
        player entity-y @
        at-xy process-input
    haltgame @ until
;

player
    '@' 0 0
    <A White >FG A>
entity!

vid-clear
fov-recompute on
player 6 10 30 generate-map
player add-entity

mainloop
0 attr!
bye
