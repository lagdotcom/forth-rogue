include defs.fs
include vars.fs
include utils.fs
include rect.fs
include queue.fs
include entity.fs
include map.fs
include fov.fs
include vid.fs
include bfs.fs
include fighter.fs
include ai.fs
include mapgen.fs

: move-player ( mx my -- flag )
    over over                   ( mx my mx my )
    player entity-y @ +         ( mx my mx y )
    player entity-x @ under+    ( mx my x y )

    2dup get-blocker dup if
        nip nip
        player swap attack
        2drop true exit
    else drop then

    map-passable if
        player clear-entity
        player move-entity
        fov-recompute on
        true
    else
        2drop
        false
    then
;

: process-input ( -- flag )
    ekey
    case
        k-esc   of haltgame on false endof
        'q'     of haltgame on false endof

        k-up    of  0 -1 move-player endof
        k-right of  1  0 move-player endof
        k-down  of  0  1 move-player endof
        k-left  of -1  0 move-player endof

        k-home  of -1 -1 move-player endof
        k-prior of  1 -1 move-player endof
        k-next  of  1  1 move-player endof
        k-end   of -1  1 move-player endof

        \ unrecognised key; don't use up player turn
        false swap
    endcase
;

: render-all ( -- )
    clear-all-entities
    fov-recompute if recompute-fov then
    render-map
    fov-recompute off
    draw-all-entities
    present
;

: handle-player-turn ( -- flag )
    player entity-xy@
    at-xy process-input
;

: call-ai ( entity -- )
    dup entity-ai @ dup if      ( entity ai )
        ai-fn @ execute
    else 2drop then
;

: handle-enemy-turn ( -- )
    ['] call-ai for-each-entity
;

: mainloop ( -- )
    haltgame off
    begin
        render-all
        handle-player-turn      ( used )
        if handle-enemy-turn then
    haltgame @ until
;

player
    '@' 0 0
    <A White >FG A>
    s" you"
    ENTITY_BLOCKS
entity!
player 30 2 5 add-fighter

vid-clear
fov-recompute on
player 6 10 30 2 generate-map
player add-entity

mainloop

free-all-entities
0 attr!
bye
