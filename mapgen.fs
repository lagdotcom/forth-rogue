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

: carve-random-tunnel { _x1 _y1 _x2 _y2 -- }
    0 1 randint if
        _x1 _x2 _y1 carve-h-tunnel
        _y1 _y2 _x2 carve-v-tunnel
    else
        _y1 _y2 _x1 carve-v-tunnel
        _x1 _x2 _y2 carve-h-tunnel
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

: add-down-stairs ( x y -- )
    [char] > -rot
    white
    c" stairs"
    LAYER_STAIRS
    ENTITY_SHOULD_REVEAL
    alloc-entity dup add-entity
    dungeon-level 1+ add-stairs
;

: lookup-item-use ( x -- str u )
    case
        'use-healing-potion of s" 'use-healing-potion" endof
        'use-lightning-scroll of s" 'use-lightning-scroll" endof
        'use-fireball-scroll of s" 'use-fireball-scroll" endof
        'use-confusion-scroll of s" 'use-confusion-scroll" endof

        s" 0" rot
    endcase
;

: place-monster-in-room ( -- )
    gen-x1 1+ gen-x2 2 - randint
    gen-y1 1+ gen-y2 2 - randint         ( x y )
    2dup get-blocker if
        2drop exit
    then

    get-random-monster execute
;

: place-item-in-room ( -- )
    gen-x1 1+ gen-x2 2 - randint
    gen-y1 1+ gen-y2 2 - randint         ( x y )
    2dup get-entity-at if
        2drop exit
    then

    get-random-item execute
;

table: get-max-monsters ( floor -- max )
    4 2 rawentry
    6 3 rawentry
   -1 4 rawentry
endtable

table: get-max-items ( floor -- max )
    4 1 rawentry
   -1 2 rawentry
endtable

: generate-room { _min _max -- }
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

    0 dungeon-level get-max-monsters randint 0 ?do
        place-monster-in-room
    loop

    0 dungeon-level get-max-items randint 0 ?do
        place-item-in-room
    loop
;

0 value map-seed
0 value map-min
0 value map-max
0 value map-rooms
: generate-map { _player _min _max _rooms -- }
    seed @ to map-seed
    _min to map-min
    _max to map-max
    _rooms to map-rooms
    <log
        s" -- generating map, seed=" logtype
        map-seed log.
    log>

    rect% _rooms * %alloc to gen-rects
    0 to gen-numrects

    1 fill-map

    _rooms 0 ?do
        _min _max generate-room
    loop

    gen-rects rect@ rect-centre
    _player entity-y !
    _player entity-x !

    gen-rects gen-numrects 1- rect-size * +
    rect@ rect-centre add-down-stairs

    gen-rects free throw
;
