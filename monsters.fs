: add-orc ( x y -- )
    [char] o -rot
    green
    c" orc"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    10 0 4 35 add-fighter
    apply-basic-ai
;

: add-troll ( x y -- )
    [char] T -rot
    light-green
    c" troll"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    16 2 8 100 add-fighter
    apply-basic-ai
;

: get-random-monster-1 ( -- xt )
    ['] add-orc
;

randtable: get-random-monster-3 ( -- xt )
    80 entry: add-orc
    15 entry: add-troll
endtable

randtable: get-random-monster-5 ( -- xt )
    80 entry: add-orc
    30 entry: add-troll
endtable

randtable: get-random-monster-7 ( -- xt )
    80 entry: add-orc
    60 entry: add-troll
endtable

table: get-random-monster-group ( -- xt )
    3 ' get-random-monster-1 rawentry
    5 ' get-random-monster-3 rawentry
    7 ' get-random-monster-5 rawentry
   -1 ' get-random-monster-7 rawentry
endtable
: get-random-monster ( -- xt )
    dungeon-level get-random-monster-group execute
;
