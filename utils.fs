: randint ( min max -- n )
    1+ over - random +
;

: maybe-free ( addr -- )
    dup @ dup if            ( addr obj )
        free throw          ( addr )
        0 swap !
    else 2drop then
;
