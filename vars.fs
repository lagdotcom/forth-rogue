1 constant TILE_BLOCKED
2 constant TILE_OPAQUE
4 constant TILE_EXPLORED

1 constant ENTITY_BLOCKS

27 constant k-esc

<A Blue >BG A>          constant dark-wall
<A Blue >BG Blink A>    constant dark-ground
<A Red >BG A>           constant light-wall
<A Red >BG Blink A>     constant light-ground

form 1- * constant vidbuf-size
variable vidbuf-fg      vidbuf-size allot
variable vidbuf-bg      vidbuf-size allot
variable vidbuf-ch      vidbuf-size allot
variable vidbuf-dirty   vidbuf-size allot

60 constant map-width
30 constant map-height

map-width map-height * constant map-tiles
variable game-map map-tiles allot

10 constant fov-radius
variable fov-recompute
variable fov-map map-tiles allot

variable haltgame
create player entity-size allot
