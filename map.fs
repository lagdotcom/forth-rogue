: map-offset ( x y -- offset )
    map-width * game-map + +
;

: map-contains ( x y -- flag )
    0 map-height within 0= if
        drop false exit
    then
    0 map-width within 0= if
        false exit
    else true then
;

: map-passable ( x y -- flag )
    2dup map-contains if
        map-offset c@ TILE_BLOCKED and 0=
    else 2drop false then
;

: fdistance { _x1 _y1 _x2 _y2 -- F: dist }
    _x1 _x2 - dup *
    _y1 _y2 - dup *
    +
    s>d d>f fsqrt
;
