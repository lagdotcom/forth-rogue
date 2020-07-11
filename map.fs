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

: fdistance { x1 y1 x2 y2 -- F: dist }
    x1 x2 - dup *
    y1 y2 - dup *
    +
    s>d d>f fsqrt
;
