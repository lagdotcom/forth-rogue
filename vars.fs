1 constant TILE_BLOCKED
2 constant TILE_OPAQUE
4 constant TILE_EXPLORED

1 constant ENTITY_BLOCKS
2 constant ENTITY_NAME_ALLOC

1 constant LAYER_CORPSE
2 constant LAYER_ITEM
3 constant LAYER_ENEMY
10 constant LAYER_PLAYER

27 constant k-esc

<A Green >BG A>         constant bg-green
<A Green >BG Blink A>   constant bg-light-green
<A Blue >BG A>          constant bg-blue
<A Blue >BG Blink A>    constant bg-light-blue
<A Red >BG A>           constant bg-red
<A Red >BG Blink A>     constant bg-light-red

<A White >FG A>         constant fg-white
<A Green >FG A>         constant fg-green
<A Red >FG A>           constant fg-red

bg-blue constant dark-wall
bg-light-blue constant dark-ground
bg-red constant light-wall
bg-light-red constant light-ground

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

20 constant bar-width
rows 1- map-height - 10 min constant msg-log-size
bar-width 2 + constant msg-log-x
cols 1- msg-log-x - constant msg-log-w
map-height 2 + constant msg-log-y
