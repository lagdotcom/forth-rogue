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
    cell% field entity-stairs
    cell% field entity-level
    cell% field entity-equippable
    cell% field entity-equipment
end-struct entity%
entity% %size constant entity-size

: entity-xy@ ( en -- x y )
     dup entity-x @
    swap entity-y @
;

: entity-xy! { _x _y _en -- }
    _x _en entity-x !
    _y _en entity-y !
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
    cell% field fighter-xp
    cell% field fighter-power
    cell% field fighter-defense
    cell% field fighter-max-hp
    cell% field fighter-hp
end-struct fighter%

struct
    cell% field ai-fn           ( entity -- )
    cell% field ai-free-fn      ( ai -- )
    cell% field ai-data0
    cell% field ai-data1
end-struct ai%

struct
    cell% field inventory-capacity
    cell% field inventory-items
end-struct inventory%

struct
    cell% field item-use        ( entity -- flag )
end-struct item%

struct
    cell% field stairs-floor
end-struct stairs%

struct
    cell% field level-current
    cell% field level-xp
    cell% field level-base
    cell% field level-factor
end-struct level%

struct
    cell% field equippable-slot
    cell% field equippable-power-bonus
    cell% field equippable-defense-bonus
    cell% field equippable-hp-bonus
end-struct equippable%
3 constant equippable-bonuses

struct
    cell% field equipment-main-hand
    cell% field equipment-off-hand
end-struct equipment%
2 constant equipment-slots

defer add-xp
defer choose-level-bonus
defer clear-entity
defer free-entity
defer get-defense
defer get-item-target
defer get-max-hp
defer get-power
defer maybe-free-ai
defer maybe-free-inventory
defer refresh-ui
