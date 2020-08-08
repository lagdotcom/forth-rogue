:noname ( entity -- flag )
    10 item-heal
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
    20 5 item-lightning
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
    get-item-target if 12 3 item-fireball
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

randtable: get-random-item  ( x y -- )
    70 entry: add-healing-potion
    10 entry: add-lightning-scroll
    10 entry: add-fireball-scroll
    10 entry: add-confusion-scroll
endtable
