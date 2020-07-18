\ true constant debug-allocations
\ true constant debug-bfs
\ true constant debug-entity

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
include rect.fs
include queue.fs
include entity.fs
include actions.fs
include msgbuf.fs
include announce.fs
include map.fs
include fov.fs
include vid.fs
include bfs.fs
include fighter.fs
include ai.fs
include inventory.fs
include item.fs
include mapgen.fs
include keys.fs
include menu.fs
s" --- included all deps" logwriteln

\ in some versions of gforth, the seed isn't initialised
[IFUNDEF] seed-init
    utime drop seed +! rnd drop
[THEN]

variable cursor-active  false cursor-active !
variable cursor-x
variable cursor-y
: cursor-bounds-check ( -- )
    cursor-x @ 0 max map-width  min cursor-x !
    cursor-y @ 0 max map-height min cursor-y !
;
: cursor-xy+! ( x y -- )
    cursor-y +!
    cursor-x +!
    cursor-bounds-check
;
: cursor-xy! ( x y -- )
    cursor-y !
    cursor-x !
    cursor-bounds-check
;
: cursor-xy@ ( -- x y )
    cursor-x @ cursor-y @
;

: player-dead? ( -- flag )
    player entity-fighter @
    fighter-hp @ 1 <
;

: move-cursor ( mx my -- 0 )
    cursor-active @ if cursor-xy+! else
        cursor-active on
        player entity-xy@ cursor-xy!
        cursor-xy+!
    then false
;

: move-player ( mx my -- flag )
    cursor-active off
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

false value continue-getting-items
variable got-item-count
: try-get-item { _en -- }
    continue-getting-items if
        player entity-xy@ _en entity-xy@ d= if
            _en entity-item @ if
                _en player swap add-to-inventory if
                    _en announce-player-got-item
                    1 got-item-count +!
                    _en remove-entity
                else
                    false to continue-getting-items
                    announce-inventory-full
                then
            then
        then
    then
;

: get-items-at-player ( -- )
    0 got-item-count !
    true to continue-getting-items
    ['] try-get-item for-each-entity

    got-item-count @ 0= continue-getting-items and if
        announce-get-failed false
    else true then
;

: add-inventory-string { _en _index -- c-addr }
    <m
        [char] ( memit
        _index [char] a + memit
        s" ) " mtype
        _en entity-name@ mtype
    m> _index add-menu-item
;

false value input-processor
: process-input ( -- flag )
    input-processor execute
;

defer show-inventory
:noname ( -- flag )
    \ TODO: numpad 5 counts as k-esc ???
    ekey ekey>char if       \ normal key
        case
            \ k-esc         of haltgame on false endof
            k-q           of haltgame on false endof
            k-g           of get-items-at-player endof
            k-i           of show-inventory false endof

            k-shift-8     of  0 -1 move-cursor endof
            k-shift-6     of  1  0 move-cursor endof
            k-shift-2     of  0  1 move-cursor endof
            k-shift-4     of -1  0 move-cursor endof

            k-8           of  0 -1 move-player endof
            k-6           of  1  0 move-player endof
            k-2           of  0  1 move-player endof
            k-4           of -1  0 move-player endof

            \ unrecognised key; don't use up player turn
            false swap
        endcase
    else ekey>fkey if       \ meta key
        case
            k-shift-up    of  0 -1 move-cursor endof
            k-shift-right of  1  0 move-cursor endof
            k-shift-down  of  0  1 move-cursor endof
            k-shift-left  of -1  0 move-cursor endof

            k-up          of  0 -1 move-player endof
            k-right       of  1  0 move-player endof
            k-down        of  0  1 move-player endof
            k-left        of -1  0 move-player endof

            \ unrecognised key; don't use up player turn
            false swap
        endcase
    else                    \ unknown event type
        drop false
    then then
; constant 'player-turn-input

false value menu-callback
: reset-input-processor ( -- )
    false to menu-callback
    'player-turn-input to input-processor
    vid-clear
    fov-recompute on
    true to ui-update-log
;

:noname ( index -- flag )
    <log
        s" - using item index: " logtype
        dup log.
    log>

    drop    \ TODO
    reset-input-processor
    false   \ don't use up turn
; constant 'use-item-from-inventory

:noname ( -- flag )
    ekey ekey>char if
        dup [char] a [char] z within if
            [char] a - menu-callback execute exit
        then
    else drop then
    reset-input-processor false
; constant 'menu-input

: set-menu-callback ( 'fn -- )
    to menu-callback
    'menu-input to input-processor
;

:noname ( -- )
    clear-menu
    player entity-inventory @ inventory-items @
    player entity-inventory @ inventory-capacity @ 0 ?do
        dup @ ?dup-if i add-inventory-string then cell+
    loop

    s" Choose item to use. Any other button cancels." show-menu
    'use-item-from-inventory set-menu-callback
; is show-inventory

: draw-hp-bar ( -- )
    1 msg-log-y
    bar-width
    <#
        player entity-fighter @ fighter-max-hp @ s>d
        #s 2drop        \ no longer interested in this number
        [char] / hold

        player entity-fighter @ fighter-hp @ s>d
        swap over dabs  \ magic to tuck a sign byte
        #s              \ still need this number for #> to 2drop
        rot sign        \ output - if negative!
        bl hold
        [char] : hold
        [char] P hold
        [char] H hold
    #>
    player entity-fighter @ fighter-hp @
    player entity-fighter @ fighter-max-hp @
    green red
    draw-bar
;

: show-log-line { _msg _y -- }
    msg-log-x msg-log-y _y + black msg-log-w plot-spaces
    msg-log-x msg-log-y _y + white transparent _msg count plot-str
;

: draw-ui ( -- )
    draw-hp-bar

    ui-update-log if
        msg-log msg-log-size 0 ?do
            dup @ ?dup-if
                i show-log-line
            then cell+
        loop drop

        false to ui-update-log
    then
;

: render-all ( -- )
    menu-callback 0= if
        clear-all-entities
        fov-recompute if recompute-fov then
        render-map
        fov-recompute off
        draw-all-entities
        draw-ui
    then
    present
;

: handle-player-turn ( -- flag )
    cursor-active @ if
        cursor-xy@
    else
        player entity-xy@
    then

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
    char @ 0 0
    white
    get-player-name
    LAYER_PLAYER
    ENTITY_BLOCKS
entity!
player 100 2 5 add-fighter
player 26 add-inventory

page vid-clear
fov-recompute on
player 6 10 30 3 2 generate-map
player add-entity

reset-input-processor
mainloop
ansi-reset

s" --- cleanup started" logwriteln
cleanup
logclose

0 map-height at-xy
bye
