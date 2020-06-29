\ TODO: linked list?
100 constant max-entities
create entities max-entities cells allot
entities max-entities entity-size * 0 fill

: move-entity ( mx my en -- )
    tuck entity-y +! entity-x +!
;

: entity! { entity ch x y fg -- }
    ch entity entity-ch !
    x entity entity-x !
    y entity entity-y !
    fg entity entity-fg !
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
