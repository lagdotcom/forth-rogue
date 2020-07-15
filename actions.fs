100 cells queue: action-queue
action-queue clear-queue

: add-action ( action argument -- )
    action-queue enqueue
    action-queue enqueue
;

: run-actions ( -- )
    begin action-queue queue-is-empty 0= while
        action-queue dequeue
        action-queue dequeue        ( argument action )
        execute
    repeat

    action-queue clear-queue
;

:noname ( s-addr -- )
    \ copy to debug log
    dup count logwriteln
    add-to-log
; constant 'message

: <message ( -- xt str u )
    'message <m
;

: message> ( xt str u -- )
    m> add-action
;

:noname { _en -- }
    <message
        _en mname
        m"  dies!"
    message>

    _en clear-entity
    _en remove-entity

    \ make a corpse
    [char] % _en entity-xy@
    red
    <m m" remains of " _en mname m>
    LAYER_CORPSE
    ENTITY_NAME_ALLOC
    alloc-entity add-entity

    \ don't free the player, they have special considerations
    _en player <> if
        _en free-entity
    then
; constant 'entity-died

: cleanup ( -- )
    s" - freeing action queue" logwriteln
    action-queue free-queue
cleanup ;
