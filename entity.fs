\ TODO: linked list?
100 constant max-entities
max-entities cells allocate throw constant entities

: zero-entities ( -- )
    entities max-entities cells 0 fill
;
zero-entities

: move-entity ( mx my en -- )
    tuck entity-y +! entity-x +!
;

: entity! { _en _ch _x _y _fg _name _layer _flags -- }
    _en entity-size 0 fill
    _ch _en entity-ch !
    _x _en entity-x !
    _y _en entity-y !
    _fg _en entity-fg !
    _name _en entity-name !
    _layer _en entity-layer !
    _flags _en entity-flags !
;

: alloc-entity { _ch _x _y _fg _name _layer _flags -- entity }
    entity% %alloc
    dup _ch _x _y _fg _name _layer _flags entity!
;

:noname ( entity -- )
[IFDEF] debug-entity
    <log
        s" free-entity: " logtype
        dup entity-name@ logtype
        [char] @ logemit
        dup hex log. decimal
    log>
[THEN]

    dup ENTITY_NAME_ALLOC and if
        \ entity-name is alloc'd, go back one and free the whole c-addr string
        dup entity-name @ 1- free
    then
    dup entity-fighter maybe-free
    dup entity-ai maybe-free
    dup entity-inventory maybe-free-inventory
    dup entity-item maybe-free

    free throw
; is free-entity

: find-entity-offset ( en list -- list+n|0 )
    \ TODO: rewrite to use first-entity-that?
    swap                    ( list en )
    max-entities 0 ?do
        over @ over         ( list en en? en )
        = if                ( list en )
            drop unloop exit
        then
        swap cell+ swap     ( list+1 en )
    loop
    false
;

: add-entity ( en -- )
    false entities find-entity-offset
    ?dup-if                 ( en offset )
        <log
            s" - added entity: " logtype
            over entity-name@ logtype
        log>

        !
    else
        <log
            s" ! failed to add entity: " logtype
            entity-name@ logtype
        log>
    then
;

: remove-entity ( en -- )
    entities find-entity-offset
    ?dup-if
        false swap !
    else
        \ maybe an error?
    then
;

: first-entity-that { _xt -- en|0 }
    entities max-entities 0 ?do
        dup @ ?dup-if                   ( addr entity )
            dup _xt execute if           ( addr entity )
                unloop nip exit         ( entity )
            else drop then
        then
        cell+
    loop drop false
;

: for-each-entity { _xt -- }
    entities max-entities 0 ?do
        dup @ ?dup-if
            _xt execute
        then
        cell+
    loop drop
;

0 value block-check-x
0 value block-check-y
: is-blocker-at-xy { _en -- flag }
    _en entity-flags @ ENTITY_BLOCKS and if
        _en entity-xy@ block-check-x block-check-y d= if
            true exit
        then
    then false
;
: get-blocker { _x _y -- en|0 }
    _x to block-check-x
    _y to block-check-y
    ['] is-blocker-at-xy first-entity-that
;

: is-entity-at-xy { _en -- flag }
    _en entity-xy@ block-check-x block-check-y d= if true
    else false then
;
: get-entity-at { _x _y -- en|0 }
    _x to block-check-x
    _y to block-check-y
    ['] is-entity-at-xy first-entity-that
;

: free-all-entities ( -- )
    ['] free-entity for-each-entity
    zero-entities
;

: add-component { _en _'offset _align _size -- component }
[IFDEF] debug-entity
    <log
        s" add-component " logtype
        hex _en log. decimal bl logemit
        hex _'offset log. decimal bl logemit
        _align log. bl logemit
        _size log.
    log>
[THEN]

    _en _'offset execute @ dup 0= if
        drop _align _size %alloc dup
        _en _'offset execute !
    else @ then
;

: entity.name ( entity -- )
    entity-name@ type
;

: entity.debug { _en -- }
    _en entity.name ." (" _en entity-ch @ emit ." ):" cr
    ."   at (" _en entity-xy@ . . ." )" cr

    _en entity-fighter @ ?dup-if
        ."   fighter "
        dup fighter-hp @ . ." /"
        dup fighter-max-hp @ . ." hp "
        dup fighter-defense @ . ." def "
            fighter-power @ . ." pow" cr
    then
;

: cleanup ( -- )
    s" - freeing all entities" logwriteln
    free-all-entities

    s" - freeing entities array" logwriteln
    entities free throw
cleanup ;
