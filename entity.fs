struct
    cell% field entity-ch
    cell% field entity-x
    cell% field entity-y
    cell% field entity-attr
end-struct entity%
entity% nip constant entity-size

<A White >FG A> constant empty-attr

\ TODO: linked list?
100 constant max-entities
create entities max-entities cells allot
entities max-entities entity-size * 0 fill

: move-entity ( mx my en -- )
    tuck entity-y +! entity-x +!
;

: new-entity ( ch x y attr -- )
    , , , ,
;

: draw-entity ( en -- )
    dup @ swap cell+    ( ch en.x )
    dup @ swap cell+    ( ch x en.y )
    dup @ swap cell+    ( ch x y en.attr )
    @ plot
;

: clear-entity ( en -- )
    bl swap cell+       ( ch en.x )
    dup @ swap cell+    ( ch x en.y )
    @ empty-attr plot
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

: draw-all-entities ( -- )
    entities max-entities 0 do
        dup @
        dup if
            draw-entity
        else
            drop
        then
        cell+
    loop
    drop
;

: clear-all-entities ( -- )
    entities max-entities 0 do
        dup @
        dup if
            clear-entity
        else
            drop
        then
        cell+
    loop
;
