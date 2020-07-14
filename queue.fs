struct
    cell% field queue-start
    cell% field queue-deq
    cell% field queue-enq
end-struct queue%

: clear-queue { q -- }
    q queue-start @ q queue-deq !
    q queue-start @ q queue-enq !
;

: queue-is-empty { q -- flag }
    q queue-deq @
    q queue-enq @
    =
;

: enqueue { item q -- }
    item q queue-enq @ !
    cell q queue-enq +!
;

: dequeue { q -- item }
    q queue-deq @ @
    cell q queue-deq +!
;

: queue: ( size "name" -- )
    cells allocate throw            ( buffer )
    queue% %alloc dup constant      ( buffer queue )
    queue-start !
;

: free-queue ( q -- )
    dup queue-start @ free throw
    free throw
;
