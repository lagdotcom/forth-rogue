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

: enqueue { node q -- }
    node q queue-enq @ !
    cell q queue-enq +!
;

: dequeue { q -- node }
    q queue-deq @ @
    cell q queue-deq +!
;
