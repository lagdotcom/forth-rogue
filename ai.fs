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

: ai! ( ai fn -- )
    swap ai-fn !
;

: add-ai { _en _fn -- }
    _en ['] entity-ai ai% add-component
    _fn ai!
;

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

: basic-ai { _en -- }
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
;
' basic-ai constant 'basic-ai

: cleanup ( -- )
    s" - freeing AI nodemap" logwriteln
    ai-nodemap free-nodemap
cleanup ;
