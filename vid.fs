: screen-offset ( x y -- offset )
    cols * +
;

: plot-part { x y val buffer -- }
    val x y screen-offset dup   ( val offset offset )

    buffer + dup c@             ( val offset addr old-val )
    val <> if
        true rot                ( val addr true offset )
        vidbuf-dirty + c!       ( val addr )
        c!
    else
        2drop drop
    then
;

: plot-fg ( x y fg -- )
    vidbuf-fg plot-part
;

: plot-bg ( x y fg -- )
    vidbuf-bg plot-part
;

: plot-ch ( x y ch -- )
    vidbuf-ch plot-part
;

: present-offset ( offset -- )
     dup cols /mod at-xy            ( offset )
     dup vidbuf-fg + c@             ( offset fg )
    over vidbuf-bg + c@ or attr!    ( offset )
         vidbuf-ch + c@ emit
;

: present ( -- )
    vidbuf-size 0 do
        vidbuf-dirty i + c@ if
            false i vidbuf-dirty + c!
            i present-offset
        then
    loop
;

: vid-clear ( -- )
    vidbuf-bg       vidbuf-size chars 0 fill
    vidbuf-fg       vidbuf-size chars 0 fill
    vidbuf-ch       vidbuf-size chars bl fill
    vidbuf-dirty    vidbuf-size chars true fill
;

: render-map ( -- )
    fov-recompute if
        map-tiles 0 do
            i map-width /mod        ( x y )
            game-map i +            ( x y map-addr )
            fov-map i + c@          ( x y map-addr vis )
            if
                dup c@              ( x y map-addr tile )
                TILE_EXPLORED or    ( x y map-addr tile )
                tuck swap c!        ( x y tile )
                TILE_BLOCKED and if
                    light-wall plot-bg
                else
                    light-ground plot-bg
                then
            else                    ( x y map-addr )
                c@ dup TILE_EXPLORED and if
                    TILE_BLOCKED and if
                        dark-wall plot-bg
                    else
                        dark-ground plot-bg
                    then
                else
                    drop 2drop
                then
            then
        loop
    then
;

: draw-entity { en -- }
    en entity-x @
    en entity-y @
    is-in-fov if
        en entity-x @
        en entity-y @ 2dup
        en entity-ch @ plot-ch
        en entity-fg @ plot-fg
    then
;

: clear-entity { en -- }
    en entity-x @
    en entity-y @ 2dup
    bl plot-ch
    en entity-fg @ plot-fg
;

: draw-all-entities ( -- )
    entities max-entities 0 do
        dup @
        dup if
            draw-entity
        else
            drop
        then
        cell+
    loop
    drop
;

: clear-all-entities ( -- )
    entities max-entities 0 do
        dup @
        dup if
            clear-entity
        else
            drop
        then
        cell+
    loop
;
