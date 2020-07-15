struct
    cell% field queue-start
    cell% field queue-deq
    cell% field queue-enq
end-struct queue%

: clear-queue { _q -- }
    _q queue-start @ _q queue-deq !
    _q queue-start @ _q queue-enq !
;

: queue-is-empty { _q -- flag }
    _q queue-deq @
    _q queue-enq @
    =
;

: enqueue { _item _q -- }
    _item _q queue-enq @ !
    cell _q queue-enq +!
;

: dequeue { _q -- item }
    _q queue-deq @ @
    cell _q queue-deq +!
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
