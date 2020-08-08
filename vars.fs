1 constant TILE_BLOCKED
2 constant TILE_OPAQUE
4 constant TILE_EXPLORED

1 constant ENTITY_BLOCKS
2 constant ENTITY_NAME_ALLOC
4 constant ENTITY_REVEALED
8 constant ENTITY_SHOULD_REVEAL

1 constant LAYER_STAIRS
2 constant LAYER_CORPSE
3 constant LAYER_ITEM
4 constant LAYER_ENEMY
10 constant LAYER_PLAYER

27 constant k-esc

blue constant dark-wall
light-blue constant dark-ground
red constant light-wall
light-red constant light-ground

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
entity-size allocate throw value player

20 constant bar-width
rows 1- map-height - 10 min constant msg-log-size
bar-width 2 + constant msg-log-x
cols 1- msg-log-x - constant msg-log-w
map-height 2 + constant msg-log-y

30 constant cs-width
10 constant cs-height
cols 2 / cs-width 2 / - constant cs-x
rows 2 / cs-height 2 / - constant cs-y

false value ui-update-log

1 value dungeon-level
