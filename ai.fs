map-width map-height make-nodemap constant ai-nodemap

: build-ai-nodemap { _en -- }
    ai-nodemap setup-nodemap-nodes
    map-height 0 ?do
        map-width 0 ?do
            i j get-blocker ?dup-if             ( blocker )
                _en <> if
                    i j ai-nodemap at-nodemap   ( node )
                    node-blocked on
                then
            else
                i j map-passable 0= if
                    i j ai-nodemap at-nodemap
                    node-blocked on
                then
            then
        loop
    loop
;

\ this returns true if move FAILED
: move-towards-using-nodemap { _en _x _y -- flag }
    _en build-ai-nodemap

    \ make sure the destination tile is not blocked...
    _x _y ai-nodemap at-nodemap node-blocked off

    _en entity-xy@ _x _y ai-nodemap bfs if      ( x y )
        _en clear-entity
        _en entity-y !
        _en entity-x !
        false
    else true then
;

: ai! { _ai _fn _free-fn _d0 _d1 -- }
         _fn _ai ai-fn !
    _free-fn _ai ai-free-fn !
         _d0 _ai ai-data0 !
         _d1 _ai ai-data1 !
;

: add-ai { _en _fn _free-fn _d0 _d1 -- }
    _en ['] entity-ai ai% add-component
    _fn _free-fn _d0 _d1 ai!
;

:noname ( addr -- )
    dup @ ?dup-if                           ( addr ai )
        dup ai-free-fn @ ?dup-if            ( addr ai free-fn )
            execute                         ( addr )
        else drop then
        maybe-free
    else drop then
; is maybe-free-ai

: move-towards { _en _x _y -- }
    _x _en entity-x @ - sgn     ( mx )
    _y _en entity-y @ - sgn     ( mx my )

    over _en entity-x @ +       ( mx my dx )
    over _en entity-y @ +       ( mx my dx dy )

    2dup get-blocker if
        2drop 2drop exit
    then

    map-passable if
        _en dup clear-entity move-entity
    else 2drop then
;

:noname { _en -- }
    _en entity-xy@ is-in-fov if
        _en entity-xy@
        player entity-xy@

        distance 1 = if
            _en player attack
        else
            _en
            player entity-xy@
            move-towards-using-nodemap if
                _en player entity-xy@ move-towards
            then
        then
    then
; constant 'basic-ai

: apply-basic-ai ( en -- )
    dup entity-ai maybe-free-ai
    'basic-ai 0 0 0 add-ai
;

\ confused ai: data0 contains previous ai, data1 contains duration

:noname { _en -- }
    _en entity-ai @ ai-data1 @ 0> if
        _en _en entity-xy@          ( en ox oy )
        swap -1 1 randint +         ( en oy dx )
        swap -1 1 randint +         ( en dx dy )
        move-towards

        -1 _en entity-ai @ ai-data1 +!
    else
        _en entity-ai @ dup         ( ai ai )
        ai-data0 @ _en entity-ai !  ( ai )
        free throw
        _en announce-confusion-over
    then
; constant 'confused-ai

:noname ( ai -- )
    ai-data0 maybe-free
; constant 'confused-ai-free

: apply-confused-ai { _en _duration -- }
    _en
    'confused-ai
    'confused-ai-free
    _en entity-ai @
        0 _en entity-ai !       \ zero old ai
    _duration
    add-ai
;

: cleanup ( -- )
    s" - freeing AI nodemap" logwriteln
    ai-nodemap free-nodemap
cleanup ;
