include defs.fs
include vars.fs
include utils.fs
include rect.fs
include queue.fs
include entity.fs
include message.fs
include actions.fs
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

: player-dead? ( -- flag )
    player entity-fighter @
    fighter-hp @ 1 <
;

: process-input ( -- flag )
    \ TODO: numpad 5 counts as k-esc ???

    ekey
    case
        k-esc   of haltgame on false endof
        'q'     of haltgame on false endof

        k-up    of  0 -1 move-player endof
        k-right of  1  0 move-player endof
        k-down  of  0  1 move-player endof
        k-left  of -1  0 move-player endof

        \ unrecognised key; don't use up player turn
        false swap
    endcase
;

: draw-ui ( -- )
    1
    map-height 2 +
    <A White >FG A>
    <A Red >BG A>
    <#
        player entity-fighter @ fighter-max-hp @ s>d
        # # # 2drop     \ no longer interested in this number
        '/' hold

        player entity-fighter @ fighter-hp @ s>d
        # # #           \ still need this number for #> to 2drop
        bl hold
        ':' hold
        'P' hold
        'H' hold
    #>
    plot-str
;

: render-all ( -- )
    clear-all-entities
    fov-recompute if recompute-fov then
    render-map
    fov-recompute off
    draw-all-entities
    draw-ui
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
        run-actions
        if
            handle-enemy-turn
            run-actions
        then

        player-dead? if
            \ TODO
            haltgame on
        then
    haltgame @ until
;

player
    '@' 0 0
    <A White >FG A>
    s" player"
    ENTITY_BLOCKS
entity!
player 10 2 5 add-fighter

vid-clear
fov-recompute on
player 6 10 30 2 generate-map
player add-entity

mainloop

free-all-entities
0 attr!
bye
