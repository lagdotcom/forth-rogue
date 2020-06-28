1 constant TILE_BLOCKED
2 constant TILE_OPAQUE

60 constant map-width
30 constant map-height
map-width map-height * constant map-tiles

<A Blue >BG A>          constant dark-wall
<A Blue >BG Blink A>    constant dark-ground

variable game-map map-tiles allot
game-map map-tiles 0 fill

: map-offset ( x y -- offset )
    map-width * game-map + +
;

: map-passable ( x y -- flag )
    map-offset c@ TILE_BLOCKED and 0=
;

: render-map ( -- )
    map-tiles 0 do
        bl                  ( ch )
        i map-width /mod    ( ch x y )
        game-map i + c@     ( ch x y flags )
        TILE_BLOCKED and if
            dark-wall
        else
            dark-ground
        then
        plot
    loop
;

TILE_BLOCKED TILE_OPAQUE or
dup 30 22 map-offset !
dup 31 22 map-offset !
    32 22 map-offset !
