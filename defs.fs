struct
    cell% field entity-ch
    cell% field entity-x
    cell% field entity-y
    cell% field entity-fg
    cell% field entity-name
    cell% field entity-layer            \ TODO: does nothing yet
    cell% field entity-flags
    cell% field entity-fighter
    cell% field entity-ai
    cell% field entity-inventory
    cell% field entity-item
end-struct entity%
entity% %size constant entity-size

: entity-xy@ ( en -- x y )
     dup entity-x @
    swap entity-y @
;

: entity-name@ ( en -- str u )
    entity-name @ count
;

struct
    char% field rect-x1
    char% field rect-y1
    char% field rect-x2
    char% field rect-y2
end-struct rect%
rect% %size constant rect-size

struct
    cell% field fighter-max-hp
    cell% field fighter-hp
    cell% field fighter-defense
    cell% field fighter-power
end-struct fighter%

struct
    cell% field ai-fn           ( entity -- )
end-struct ai%

struct
    cell% field inventory-capacity
    cell% field inventory-items
end-struct inventory%

struct
    cell% field item-flags
    cell% field item-use-fn
end-struct item%

defer clear-entity
defer maybe-free-inventory
