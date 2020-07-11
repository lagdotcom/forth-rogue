: ai! { ai fn -- }
    fn ai ai-fn !
;

: add-ai { entity fn -- }
    entity ['] entity-ai ai-size add-component
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
    entity entity-xy@
    is-in-fov if
        entity entity-xy@
        player entity-xy@

        fdistance 2e f< if
            \ TODO: attack!
        else
            entity
            player entity-xy@
            move-towards
        then
    then
;
' basic-ai constant 'basic-ai
