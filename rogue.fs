\ true constant debug-allocations
\ true constant debug-bfs

include debuglog.fs
s" --- forth rogue v0.1 starting up" logwriteln

\ this will be extended by deps
: cleanup ( -- )
    s" --- cleanup finished" logwriteln
;

include compat.fs
include random.fs       \ gforth library - simple RNG
include ansi.fs
include defs.fs
include vars.fs
include utils.fs
include message.fs
include rect.fs
include queue.fs
include entity.fs
include actions.fs
include map.fs
include fov.fs
include vid.fs
include bfs.fs
include fighter.fs
include ai.fs
include mapgen.fs
s" --- included all deps" logwriteln

: player-dead? ( -- flag )
    player entity-fighter @
    fighter-hp @ 1 <
;

: move-player ( mx my -- flag )
    player-dead? if 2drop false exit then

    over over                   ( mx my mx my )
    player entity-y @ +         ( mx my mx y )
    player entity-x @ under+    ( mx my x y )

    2dup get-blocker ?dup-if
        nip nip
        player swap attack
        2drop true exit
    then

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
    \ TODO: numpad 5 counts as k-esc ???

    ekey ekey>char if       \ normal key
        case
            \ k-esc   of haltgame on false endof
            'q'     of haltgame on false endof
            '8'     of  0 -1 move-player endof
            '6'     of  1  0 move-player endof
            '2'     of  0  1 move-player endof
            '4'     of -1  0 move-player endof

            \ unrecognised key; don't use up player turn
            false swap
        endcase
    else ekey>fkey if       \ meta key
        case
            k-up    of  0 -1 move-player endof
            k-right of  1  0 move-player endof
            k-down  of  0  1 move-player endof
            k-left  of -1  0 move-player endof

            \ unrecognised key; don't use up player turn
            false swap
        endcase
    else                    \ unknown event type
        drop false
    then then
;

: draw-hp-bar ( -- )
    1 msg-log-y
    bar-width
    <#
        player entity-fighter @ fighter-max-hp @ s>d
        #s 2drop        \ no longer interested in this number
        '/' hold

        player entity-fighter @ fighter-hp @ s>d
        swap over dabs  \ magic to tuck a sign byte
        #s              \ still need this number for #> to 2drop
        rot sign        \ output - if negative!
        bl hold
        ':' hold
        'P' hold
        'H' hold
    #>
    player entity-fighter @ fighter-hp @
    player entity-fighter @ fighter-max-hp @
    green red
    draw-bar
;

: show-log-line { msg y -- }
    msg-log-x msg-log-y y + 0 msg-log-w plot-spaces
    msg-log-x msg-log-y y + white 0 msg count plot-str
;

: draw-ui ( -- )
    draw-hp-bar

    msg-log msg-log-size 0 ?do
        dup @ ?dup-if
            i show-log-line
        then cell+
    loop drop
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
    dup entity-ai @ ?dup-if      ( entity ai )
        ai-fn @ execute
    else drop then
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
    haltgame @ until
;

: get-player-name ( -- c-addr )
    c" player"
;

player
    '@' 0 0
    white
    get-player-name
    LAYER_PLAYER
    ENTITY_BLOCKS
entity!
player 10 2 5 add-fighter

vid-clear
fov-recompute on
player 6 10 30 2 generate-map
player add-entity

mainloop

s" --- cleanup started" logwriteln
cleanup
logclose

0 map-height at-xy
ansi-reset
bye
