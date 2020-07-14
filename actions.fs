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
    \ TODO: scrolling etc.
    dup count type cr   ( s-addr )
    free throw
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
    '%' entity entity-x @ entity entity-y @
    <A Red >FG A>
    <m m" remains of " entity mname m> count
    ENTITY_NAME_ALLOC
    alloc-entity add-entity

    entity free-entity
; constant 'entity-died
