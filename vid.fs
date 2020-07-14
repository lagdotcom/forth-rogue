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

: plot-bg ( x y bg -- )
    vidbuf-bg plot-part
;

: plot-ch ( x y ch -- )
    vidbuf-ch plot-part
;

: plot-str { x y fg bg str str-len -- }
    str-len 0 ?do
              x i + y fg plot-fg
        bg if x i + y bg plot-bg then
              x i + y str i + c@ plot-ch
    loop
;

: plot-spaces { x y bg count -- }
    count 0 ?do
        x i + y 2dup
            bg plot-bg
            bl plot-ch
    loop
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
    en entity-xy@
    is-in-fov if
    \ <log
    \     s" drawing entity: " logtype
    \     en entity-name@ logtype
    \ log>
        en entity-xy@ 2dup
        en entity-ch @ plot-ch
        en entity-fg @ plot-fg
    then
;

:noname { en -- }
    en entity-xy@ 2dup
    bl plot-ch
    en entity-fg @ plot-fg
; is clear-entity

: draw-all-entities ( -- )
    ['] draw-entity for-each-entity
;

: clear-all-entities ( -- )
    ['] clear-entity for-each-entity
;

: draw-bar { x y width name name-len val vmax fill back -- }
    x y back width              plot-spaces
    val 0> if
        x y fill val width * vmax / plot-spaces
    then
    x 1+ y 0 0 name name-len    plot-str
;
