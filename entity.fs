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

: entity-xy@ ( entity -- x y )
    dup entity-x @ swap entity-y @
;

: entity! { entity ch x y fg name name-len flags -- }
    entity entity-size 0 fill
    ch entity entity-ch !
    x entity entity-x !
    y entity entity-y !
    fg entity entity-fg !
    name entity entity-name !
    name-len entity entity-name-len !
    flags entity entity-flags !
;

: alloc-entity { ch x y fg name name-len flags -- entity }
    entity-size allocate throw
    dup ch x y fg name name-len flags entity!
;

: free-entity ( entity -- )
    dup entity-fighter maybe-free
    dup entity-ai maybe-free

    free throw
;

: find-entity-offset ( en list -- list+n|0 )
    swap                    ( list en )
    max-entities 0 do
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
    dup if                  ( en offset )
        !
    else
        \ log error or something LOL
        drop
    then
;

: remove-entity ( en -- )
    entities find-entity-offset
    dup if
        false swap !
    else
        \ maybe an error?
    then
    drop
;

: get-blocker { x y -- en|0 }
    entities max-entities 0 do
        dup @ dup if
            dup entity-flags @ ENTITY_BLOCKS and if
                entity-xy@ x y d= if
                    unloop exit
                then
            else drop then
        else drop then
        cell+
    loop drop false
;

: for-each-entity { xt -- }
    entities max-entities 0 do
        dup @
        dup if
            xt execute
        else
            drop
        then
        cell+
    loop
    drop
;

: free-all-entities ( -- )
    ['] free-entity for-each-entity
    zero-entities
;

: add-component { entity 'offset size -- component }
    entity 'offset execute @ dup 0= if
        drop size allocate throw dup
        entity 'offset execute !
    else @ then
;
