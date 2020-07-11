: fill-map ( tile -- )
    >r game-map map-tiles r> fill
;

: carve-rect { x1 y1 x2 y2 -- }
    x2 1- x1 1+ do
        y2 1- y1 1+ do
            0 j i map-offset c!
        loop
    loop
;

: carve-h-tunnel { x1 x2 y -- }
    x1 x2 > if
        x1 1+ x2
    else
        x2 1+ x1
    then ?do
        0 i y map-offset c!
    loop
;

: carve-v-tunnel { y1 y2 x -- }
    y1 y2 > if
        y1 1+ y2
    else
        y2 1+ y1
    then ?do
        0 x i map-offset c!
    loop
;

: carve-random-tunnel { prv-x prv-y new-x new-y -- }
    0 1 randint if
        prv-x new-x prv-y carve-h-tunnel
        prv-y new-y new-x carve-v-tunnel
    else
        prv-y new-y prv-x carve-v-tunnel
        prv-x new-x new-y carve-h-tunnel
    then
;

0 value gen-x1
0 value gen-y1
0 value gen-x2
0 value gen-y2
0 value gen-w
0 value gen-h
0 value gen-cx
0 value gen-cy

: add-orc ( x y -- )
    'o' -rot
    <A Green >FG A>
    s" orc"
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    10 0 3 add-fighter
    'basic-ai add-ai
;

: add-troll ( x y -- )
    'T' -rot
    <A Green >FG A>
    s" troll"
    ENTITY_BLOCKS
    alloc-entity dup dup add-entity
    16 1 4 add-fighter
    'basic-ai add-ai
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

: generate-room { rects numrects min-size max-size max-monsters -- rects numrects }
    min-size max-size randint to gen-w
    min-size max-size randint to gen-h
    0 map-width gen-w - randint
    0 map-height gen-h - randint
    gen-w gen-h rect-convert
    to gen-y2 to gen-x2 to gen-y1 to gen-x1

    rects dup numrects 0 ?do            ( orects rects )
        dup rect@ gen-x1 gen-y1 gen-x2 gen-y2           ( orects rects r1 r2 )
        rect-intersects if              ( orects rects )
            drop numrects
            unloop exit
        then

        rect-size +
    loop                                ( orects rects )

    gen-x1 gen-y1 gen-x2 gen-y2 rect-centre
    to gen-cy to gen-cx
    gen-x1 gen-y1 gen-x2 gen-y2 rect!
    gen-x1 gen-y1 gen-x2 gen-y2 carve-rect

    numrects dup 0> if
        2dup 1- rect-size * + rect@ rect-centre         ( rects numrects ox oy )
        gen-cx gen-cy carve-random-tunnel
    then 1+

    0 max-monsters randint 0 ?do
        place-monster-in-room
    loop
;

: generate-map { player min-size max-size num-rooms max-monsters -- }
    rect-size num-rooms *                   ( memsz )
    allocate throw 0                        ( rects numrects )

    1 fill-map

    num-rooms 0 do
        min-size max-size max-monsters generate-room
    loop

    drop
    dup rect@ rect-centre
    player entity-y !
    player entity-x !

    free throw
;
