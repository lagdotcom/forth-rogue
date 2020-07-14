\ TODO: linked list?
100 constant max-entities
create entities max-entities cells allot

: zero-entities ( -- )
    entities max-entities cells 0 fill
;
zero-entities

: move-entity ( mx my en -- )
    tuck entity-y +! entity-x +!
;

: entity! { entity ch x y fg name layer flags -- }
    entity entity-size 0 fill
    ch entity entity-ch !
    x entity entity-x !
    y entity entity-y !
    fg entity entity-fg !
    name entity entity-name !
    layer entity entity-layer !
    flags entity entity-flags !
;

: alloc-entity { ch x y fg name layer flags -- entity }
    entity% %alloc
    dup ch x y fg name layer flags entity!
;

: free-entity ( entity -- )
    dup ENTITY_NAME_ALLOC and if
        \ entity-name is alloc'd, go back one and free the whole c-addr string
        dup entity-name @ 1- free
    then
    dup entity-fighter maybe-free
    dup entity-ai maybe-free

    free throw
;

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

: first-entity-that { xt -- en|0 }
    entities max-entities 0 ?do
        dup @ ?dup-if                   ( addr entity )
            dup xt execute if           ( addr entity )
                unloop nip exit         ( entity )
            else drop then
        then
        cell+
    loop drop false
;

: for-each-entity { xt -- }
    entities max-entities 0 ?do
        dup @ ?dup-if
            xt execute
        then
        cell+
    loop drop
;

0 value block-check-x
0 value block-check-y
: is-blocker-at-xy { entity -- flag }
    entity entity-flags @ ENTITY_BLOCKS and if
        entity entity-xy@ block-check-x block-check-y d= if
            true exit
        then
    then false
;
: get-blocker { x y -- en|0 }
    x to block-check-x
    y to block-check-y
    ['] is-blocker-at-xy first-entity-that
;

: free-all-entities ( -- )
    ['] free-entity for-each-entity
    zero-entities
;

: add-component { entity 'offset align size -- component }
    entity 'offset execute @ dup 0= if
        drop align size %alloc dup
        entity 'offset execute !
    else @ then
;

: entity.name ( entity -- )
    entity-name@ type
;

: entity.debug { entity -- }
    entity entity.name ." (" entity entity-ch @ emit ." ):" cr
    ."   at (" entity entity-xy@ . . ." )" cr

    entity entity-fighter @ ?dup-if
        ."   fighter "
        dup fighter-hp @ . ." /"
        dup fighter-max-hp @ . ." hp "
        dup fighter-defense @ . ." def "
            fighter-power @ . ." pow" cr
    then
;

: cleanup ( -- )
    free-all-entities
    \ entities is static allocated, can't free
cleanup ;
