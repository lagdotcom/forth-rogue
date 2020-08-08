: add-orc ( x y -- )
    [char] o -rot
    green
    c" orc"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    10 0 3 35 add-fighter
    apply-basic-ai
;

: add-troll ( x y -- )
    [char] T -rot
    light-green
    c" troll"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    16 1 4 100 add-fighter
    apply-basic-ai
;

randtable: get-random-monster   ( x y -- )
    80 entry: add-orc
    20 entry: add-troll
endtable
