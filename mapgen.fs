: fill-map ( tile -- )
    >r game-map map-tiles r> fill
;

: carve-rect { _x1 _y1 _x2 _y2 -- }
    _x2 1- _x1 1+ do
        _y2 1- _y1 1+ do
            0 j i map-offset c!
        loop
    loop
;

: carve-h-tunnel { _x1 _x2 _y -- }
    _x1 _x2 > if
        _x1 1+ _x2
    else
        _x2 1+ _x1
    then ?do
        0 i _y map-offset c!
    loop
;

: carve-v-tunnel { _y1 _y2 _x -- }
    _y1 _y2 > if
        _y1 1+ _y2
    else
        _y2 1+ _y1
    then ?do
        0 _x i map-offset c!
    loop
;

: carve-random-tunnel { _px _py _nx _ny -- }
    0 1 randint if
        _px _nx _py carve-h-tunnel
        _py _ny _nx carve-v-tunnel
    else
        _py _ny _px carve-v-tunnel
        _px _nx _ny carve-h-tunnel
    then
;

0 value gen-rects
0 value gen-numrects
0 value gen-x1
0 value gen-y1
0 value gen-x2
0 value gen-y2
0 value gen-w
0 value gen-h
0 value gen-cx
0 value gen-cy

: add-orc ( x y -- )
    [char] o -rot
    green
    c" orc"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    10 0 3 add-fighter
    'basic-ai add-ai
;

: add-troll ( x y -- )
    [char] T -rot
    light-green
    c" troll"
    LAYER_ENEMY
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    16 1 4 add-fighter
    'basic-ai add-ai
;

: use-healing-potion ( entity -- flag )
    10 item-heal
;

: add-healing-potion ( x y -- )
    [char] ! -rot
    violet
    c" healing potion"
    LAYER_ITEM
    0
    alloc-entity dup add-entity
    ['] use-healing-potion add-item
;

: use-lightning-scroll ( entity -- flag )
    20 5 item-lightning
;

: add-lightning-scroll ( x y -- )
    [char] # -rot
    yellow
    c" lightning scroll"
    LAYER_ITEM
    0
    alloc-entity dup add-entity
    ['] use-lightning-scroll add-item
;

: place-monster-in-room ( -- )
    gen-x1 1+ gen-x2 2 - randint
    gen-y1 1+ gen-y2 2 - randint         ( x y )
    2dup get-blocker if
        2drop exit
    then

    0 99 randint 80 <
    if add-orc else add-troll then
;

: place-item-in-room ( -- )
    gen-x1 1+ gen-x2 2 - randint
    gen-y1 1+ gen-y2 2 - randint         ( x y )
    2dup get-entity-at if
        2drop exit
    then

    0 99 randint 70 <
    if add-healing-potion else add-lightning-scroll then
;

: generate-room { _min _max _monsters _items -- }
    _min _max randint to gen-w
    _min _max randint to gen-h
    0 map-width gen-w - randint
    0 map-height gen-h - randint
    gen-w gen-h rect-convert
    to gen-y2 to gen-x2 to gen-y1 to gen-x1

    gen-rects gen-numrects 0 ?do                        ( rect )
        dup rect@ gen-x1 gen-y1 gen-x2 gen-y2           ( rect xyxy xyxy )
        rect-intersects if                              ( rect )
            drop unloop exit
        then

        rect-size +
    loop                                                ( rect )

    <log
        s" -- adding room " logtype
        gen-x1 log.
        [char] , logemit
        gen-y1 log.
        [char] - logemit
        gen-x2 log.
        [char] , logemit
        gen-y2 log.
    log>

    gen-x1 gen-y1 gen-x2 gen-y2 rect-centre
    to gen-cy to gen-cx
    gen-x1 gen-y1 gen-x2 gen-y2 rect!
    gen-x1 gen-y1 gen-x2 gen-y2 carve-rect

    gen-numrects dup 0> if                              ( numrects )
        1- rect-size *                                  ( offset )
        gen-rects + rect@ rect-centre                   ( x y )
        gen-cx gen-cy carve-random-tunnel
    else drop then

    gen-numrects 1+ to gen-numrects

    0 _monsters randint 0 ?do
        place-monster-in-room
    loop

    0 _items randint 0 ?do
        place-item-in-room
    loop
;

: generate-map { _player _min _max _rooms _monsters _items -- }
    s" -- generating map" logwriteln

    rect% _rooms * %alloc to gen-rects
    0 to gen-numrects

    1 fill-map

    _rooms 0 ?do
        _min _max _monsters _items generate-room
    loop

    gen-rects rect@ rect-centre
    _player entity-y !
    _player entity-x !

    gen-rects free throw
;
