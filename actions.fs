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

:noname { entity -- }
    <message
        entity      mname
        m"  dies!"
    message>

    entity clear-entity
    entity remove-entity

    \ make a corpse
    [char] % entity entity-xy@
    red
    <m m" remains of " entity mname m>
    LAYER_CORPSE
    ENTITY_NAME_ALLOC
    alloc-entity add-entity

    \ don't free the player, they have special considerations
    entity player <> if
        entity free-entity
    then
; constant 'entity-died

: cleanup ( -- )
    s" - freeing action queue" logwriteln
    action-queue free-queue
cleanup ;
