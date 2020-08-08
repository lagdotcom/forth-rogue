:noname ( entity -- flag )
    40 item-heal
; constant 'use-healing-potion
: add-healing-potion ( x y -- )
    [char] ! -rot
    violet
    c" healing potion"
    LAYER_ITEM
    ENTITY_SHOULD_REVEAL
    alloc-entity dup add-entity
    'use-healing-potion add-item
;

:noname ( entity -- flag )
    40 5 item-lightning
; constant 'use-lightning-scroll
: add-lightning-scroll ( x y -- )
    [char] # -rot
    yellow
    c" lightning scroll"
    LAYER_ITEM
    ENTITY_SHOULD_REVEAL
    alloc-entity dup add-entity
    'use-lightning-scroll add-item
;

:noname ( entity -- flag )
    get-item-target if 25 3 item-fireball
    else drop false then
; constant 'use-fireball-scroll
: add-fireball-scroll ( x y -- )
    [char] # -rot
    red
    c" fireball scroll"
    LAYER_ITEM
    ENTITY_SHOULD_REVEAL
    alloc-entity dup add-entity
    'use-fireball-scroll add-item
;

:noname ( entity -- flag )
    get-item-target if 10 item-confusion
    else drop false then
; constant 'use-confusion-scroll
: add-confusion-scroll ( x y -- )
    [char] # -rot
    light-magenta
    c" confusion scroll"
    LAYER_ITEM
    ENTITY_SHOULD_REVEAL
    alloc-entity dup add-entity
    'use-confusion-scroll add-item
;

: get-random-item-1 ( -- xt )
    ['] add-healing-potion
;

randtable: get-random-item-2 ( -- xt )
    35 entry: add-healing-potion
    10 entry: add-confusion-scroll
endtable

randtable: get-random-item-4 ( -- xt )
    35 entry: add-healing-potion
    10 entry: add-confusion-scroll
    25 entry: add-lightning-scroll
endtable

randtable: get-random-item-6 ( -- xt )
    35 entry: add-healing-potion
    10 entry: add-confusion-scroll
    25 entry: add-lightning-scroll
    25 entry: add-fireball-scroll
endtable

table: get-random-item-group ( -- xt )
    2 ' get-random-item-1 rawentry
    4 ' get-random-item-2 rawentry
    6 ' get-random-item-4 rawentry
   -1 ' get-random-item-6 rawentry
endtable
: get-random-item ( -- xt )
    dungeon-level get-random-item-group execute
;
