map-width map-height make-nodemap constant ai-nodemap

: build-ai-nodemap { entity -- }
    ai-nodemap setup-nodemap-nodes
    map-height 0 ?do
        map-width 0 ?do
            i j get-blocker dup if              ( blocker )
                entity <> if
                    i j ai-nodemap at-nodemap   ( node )
                    node-blocked on
                then
            else
                drop
                i j map-passable 0= if
                    i j ai-nodemap at-nodemap
                    node-blocked on
                then
            then
        loop
    loop
;

\ this returns true if move FAILED
: move-towards-using-nodemap { entity x y -- flag }
    entity build-ai-nodemap

    \ make sure the destination tile is not blocked...
    x y ai-nodemap at-nodemap node-blocked off

    entity entity-xy@ x y ai-nodemap bfs if     ( x y )
        entity clear-entity
        entity entity-y !
        entity entity-x !
        false
    else true then
;

: ai! { ai fn -- }
    fn ai ai-fn !
;

: add-ai { entity fn -- }
    entity ['] entity-ai ai% add-component
    fn ai!
;

: move-towards { entity x y -- }
    x entity entity-x @ - sgn       ( mx )
    y entity entity-y @ - sgn       ( mx my )

    over entity entity-x @ +        ( mx my dx )
    over entity entity-y @ +        ( mx my dx dy )

    2dup get-blocker if
        2drop 2drop exit
    then

    map-passable if
        entity dup clear-entity move-entity
    else 2drop then
;

: basic-ai { entity -- }
    entity entity-xy@ is-in-fov if
        entity entity-xy@
        player entity-xy@

        fdistance 2e f< if
            entity player attack
        else
            entity
            player entity-xy@
            move-towards-using-nodemap if
                entity player entity-xy@ move-towards
            then
        then
    then
;
' basic-ai constant 'basic-ai
