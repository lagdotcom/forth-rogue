: screen-offset ( x y -- offset )
    cols * +
;

: plot-part { _x _y _val _buf -- }
    _val _x _y screen-offset dup    ( val offset offset )

    _buf + dup c@                   ( val offset addr old-val )
    _val <> if
        true rot                    ( val addr true offset )
        vidbuf-dirty + c!           ( val addr )
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

: plot-str { _x _y _fg _bg _str _str-len -- }
    _str-len 0 ?do
              _x i + _y _fg plot-fg
       _bg if _x i + _y _bg plot-bg then
              _x i + _y _str i + c@ plot-ch
    loop
;

: plot-spaces { _x _y _bg _count -- }
    _count 0 ?do
        _x i + _y 2dup
            _bg plot-bg
            bl plot-ch
    loop
;

: plot-rect { _x _y _w _h _bg -- }
    _y _h + _y ?do
        _x i _bg _w plot-spaces
    loop
;

: present-offset ( offset -- )
     dup cols /mod at-xy            ( offset )
     dup vidbuf-fg + c@ ansi-fg-256 ( offset )
     dup vidbuf-bg + c@ ansi-bg-256 ( offset )
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

: should-draw-entity { _en -- flag }
    _en entity-flags @ ENTITY_REVEALED and
    _en entity-xy@ is-in-fov
    or
;

: draw-entity { _en -- }
    _en should-draw-entity if
    \ <log
    \     s" drawing entity: " logtype
    \     en entity-name@ logtype
    \ log>
        _en entity-xy@ 2dup
        _en entity-ch @ plot-ch
        _en entity-fg @ plot-fg

        _en entity-flags @ ENTITY_SHOULD_REVEAL and if
            _en entity-flags @ ENTITY_REVEALED or
            _en entity-flags !
        then
    then
;

:noname { _en -- }
    _en entity-xy@ 2dup
    bl plot-ch
    _en entity-fg @ plot-fg
; is clear-entity

: draw-all-entities ( -- )
    ['] draw-entity for-each-entity
;

: clear-all-entities ( -- )
    ['] clear-entity for-each-entity
;

: draw-bar { _x _y _width _name _name-len _val _vmax _fill _back -- }
    _x _y _back _width plot-spaces
    _val 0> if
        _x _y _fill _val _width * _vmax / plot-spaces
    then
    _x 1+ _y white 0 _name _name-len plot-str
;
