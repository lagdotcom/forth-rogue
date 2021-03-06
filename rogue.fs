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
include stairs.fs
include equipment.fs
include equippable.fs
include item.fs
include weights.fs
include items.fs
include monsters.fs
include mapgen.fs
include keys.fs
include menu.fs
include level.fs
include save.fs
s" --- included all deps" logwriteln

\ in some versions of gforth, the seed isn't initialised
[IFUNDEF] seed-init
    utime drop seed +! rnd drop
[THEN]

variable cursor-active cursor-active off
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

:noname { _en -- }
    player _en = 0= if
        _en remove-entity
        _en free-entity
    then
; constant 'clear-old-entity
: generate-next-map ( -- )
    map-clear
    page vid-clear
    fov-recompute on
    'clear-old-entity for-each-entity
    player 6 10 30 generate-map
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

variable found-stairs
:noname { _en -- }
    player entity-xy@ _en entity-xy@ d= if
        _en entity-stairs @ ?dup-if
            player player get-max-hp 2 / heal
            stairs-floor @ to dungeon-level
            generate-next-map
            found-stairs on
            announce-used-stairs
        then
    then
; constant 'use-stairs
: try-use-stairs ( -- )
    found-stairs off
    'use-stairs for-each-entity
    found-stairs @ 0= if announce-no-stairs then
;

: add-inventory-string { _en _index -- c-addr }
    <m
        [char] ( memit
        _index [char] a + memit
        m" ) "
        _en entity-name@ mtype
        player _en is-equipped if
            _en equipment-slot@ case
                SLOT_MAIN_HAND  of m"  (in main hand)" endof
                SLOT_OFF_HAND   of m"  (in off hand)" endof
            endcase
        then
    m> _index add-menu-item
;

:noname ( -- )
    vid-clear
    fov-recompute on
    true to ui-update-log
; is refresh-ui

false value input-processor
: process-input ( -- flag )
    input-processor execute
;

: show-character-line { _y _str _str-len -- _y }
    cs-x _y white transparent _str _str-len plot-str
    _y 1+
;

: show-character-screen ( -- )
    cs-x cs-y cs-width cs-height black plot-rect
    cs-y 1 +

    s" Character Information" show-character-line 1+
    <m m" Level: " player entity-level @ level-current @ m. show-character-line
    <m m" Experience: " player entity-level @ level-xp @ m. show-character-line
    <m m" Experience to Level: " player to-next-level m. show-character-line
    <m m" Maximum HP: " player get-max-hp m. show-character-line
    <m m" Attack: " player get-power m. show-character-line
    <m m" Defense: " player get-defense m. show-character-line

    drop present
    ekey drop refresh-ui
;

s" _forthrogue.save.fs" 2constant save-filename

defer choose-item-drop
defer choose-item-use
:noname ( -- flag )
    \ TODO: numpad 5 counts as k-esc ???
    ekey ekey>char if       \ normal key
        case
            \ k-esc         of haltgame on false endof
            k-q           of haltgame on false endof

            k-d           of choose-item-drop false endof
            k-g           of get-items-at-player endof
            k-i           of choose-item-use false endof
            [char] s      of
                save-filename save-game
                announce-saved-game
                false
            endof
            k-enter       of try-use-stairs false endof
            k-c           of show-character-screen false endof

            k-shift-8     of  0 -1 move-cursor endof
            k-shift-6     of  1  0 move-cursor endof
            k-shift-2     of  0  1 move-cursor endof
            k-shift-4     of -1  0 move-cursor endof

            k-8           of  0 -1 move-player endof
            k-6           of  1  0 move-player endof
            k-2           of  0  1 move-player endof
            k-4           of -1  0 move-player endof

            k-5           of true endof
            k-z           of true endof

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
    refresh-ui
;

:noname ( index -- flag )
    <log
        s" - using item (" logtype
        dup [char] a + logemit
        [char] ) logemit
    log>

    cells player entity-inventory @ inventory-items @ +         ( eaddr )
    dup @ ?dup-if                                               ( eaddr ent )
        dup item-use@ ?dup-if                                   ( eaddr ent use )
            player swap execute if                              ( eaddr ent )
                free-entity                                     ( eaddr )
                0 swap !                                        ( )
                true                                            ( T )
            else 2drop false then                               ( F )
            reset-input-processor                               ( flag )
            exit
        else dup entity-equippable @ if
            player swap toggle-equip
            reset-input-processor
            true exit
        else
            announce-unusable-item                              ( eaddr )
        then then
    then drop false
; constant 'use-item-from-inventory

:noname ( index -- flag )
    <log
        s" - dropping item (" logtype
        dup [char] a + logemit
        [char] ) logemit
    log>

    cells player entity-inventory @ inventory-items @ +         ( eaddr )
    dup @ ?dup-if                                               ( eaddr ent )
        >r player entity-xy@                                    ( eaddr x y )
        r@ entity-y ! r@ entity-x !
        r@ add-entity
        r> announce-dropped-item
        0 swap !
        reset-input-processor true
    then drop false
; constant 'drop-item-from-inventory

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

    menu-items if
        s" Choose item to use. Any other button cancels." show-menu
        'use-item-from-inventory set-menu-callback
    else announce-no-inventory then
; is choose-item-use

:noname ( -- )
    clear-menu
    player entity-inventory @ inventory-items @
    player entity-inventory @ inventory-capacity @ 0 ?do
        dup @ ?dup-if i add-inventory-string then cell+
    loop

    menu-items if
        s" Choose item to drop. Any other button cancels." show-menu
        'drop-item-from-inventory set-menu-callback
    else announce-no-inventory then
; is choose-item-drop

: draw-hp-bar ( -- )
    1 msg-log-y
    bar-width
    <#
        player get-max-hp s>d
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
    player get-max-hp
    green red
    draw-bar
;

: show-log-line { _msg _y -- }
    msg-log-x msg-log-y _y + black msg-log-w plot-spaces
    msg-log-x msg-log-y _y +
    _msg count over @ transparent 2swap     ( fg bg str u )
    1 /string plot-str
;

: draw-ui ( -- )
    draw-hp-bar
    1 msg-log-y 2 + white transparent <m m" Floor: " dungeon-level m. plot-str
    1 msg-log-y 3 + white transparent <m m" Level: " player entity-level @ level-current @ m. plot-str

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
    then
    draw-ui
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

variable halttargeting
variable chosetarget
: process-targeting-input ( -- )
    ekey ekey>char if       \ normal key
        case
            k-q           of halttargeting on endof
            k-esc         of halttargeting on endof
            k-enter       of halttargeting on chosetarget on endof
            k-space       of halttargeting on chosetarget on endof

            k-shift-8     of  0 -1 move-cursor drop endof
            k-shift-6     of  1  0 move-cursor drop endof
            k-shift-2     of  0  1 move-cursor drop endof
            k-shift-4     of -1  0 move-cursor drop endof

            k-8           of  0 -1 move-cursor drop endof
            k-6           of  1  0 move-cursor drop endof
            k-2           of  0  1 move-cursor drop endof
            k-4           of -1  0 move-cursor drop endof
        endcase
    else ekey>fkey if       \ meta key
        case
            k-shift-up    of  0 -1 move-cursor drop endof
            k-shift-right of  1  0 move-cursor drop endof
            k-shift-down  of  0  1 move-cursor drop endof
            k-shift-left  of -1  0 move-cursor drop endof

            k-up          of  0 -1 move-cursor drop endof
            k-right       of  1  0 move-cursor drop endof
            k-down        of  0  1 move-cursor drop endof
            k-left        of -1  0 move-cursor drop endof
        endcase
    else                    \ unknown event type
        drop
    then then
;

:noname ( -- x y 1 | 0 )
    announce-targeting
    reset-input-processor
    halttargeting off
    chosetarget off
    player entity-xy@ cursor-xy!

    begin
        run-actions
        render-all
        cursor-xy@ at-xy process-targeting-input
    halttargeting @ until

    chosetarget @ if
        cursor-xy@ true
    else
        announce-targeting-cancelled false
    then
; is get-item-target

: mainloop ( -- )
    haltgame off
    begin
        render-all ansi-reset
        handle-player-turn      ( used )
        run-actions
        if
            handle-enemy-turn
            run-actions
        then
    haltgame @ until
;

: start-new-game ( -- )
    1 to dungeon-level
    player
        [char] @ 0 0
        white
        c" player"
        LAYER_PLAYER
        ENTITY_BLOCKS
    entity!
    player 100 1 2 0 add-fighter
    player 26 add-inventory
    player 1 0 200 150 add-level
    player 0 0 add-equipment

    [char] - 0 0 white c" dagger" LAYER_ITEM ENTITY_SHOULD_REVEAL
    alloc-entity
        dup SLOT_MAIN_HAND 1 0 0 add-equippable
        dup 0 add-item
        player over add-to-inventory 2drop
    player swap equip

    generate-next-map
    player add-entity

    reset-input-processor
    enable-announces
    announce-new-game run-actions
;

: save-exists ( -- flag )
    save-filename file-status drop
;

: load-game ( -- flag )
    save-exists if
        save-filename included
        refresh-ui
        reset-input-processor
        enable-announces
        announce-loaded-game run-actions
        true
    else false then
;

: show-main-menu ( -- flag )
    page clear-menu
    <m m" (n) new game" m> 0 add-menu-item
    save-exists if
        <m m" (l) load" m> 1 add-menu-item
    then
    <m m" (q) quit" m> 2 add-menu-item

    s" forthrogue" show-menu present
    begin
        ekey ekey>char if case
            [char] n of start-new-game true exit endof
            [char] l of
                load-game if true exit then
            endof
            [char] q of false exit endof
        endcase else drop then
    again
;

:noname ( -- )
    show-main-menu if
        mainloop
    then

    ansi-reset
    s" --- cleanup started" logwriteln
    cleanup
    logclose

    0 map-height at-xy
    bye
; execute
