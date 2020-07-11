include random.fs       \ gforth library - simple RNG

: randint ( min max -- n )
    1+ over - random +
;

: maybe-free ( addr -- )
    dup @ dup if            ( addr obj )
        free throw          ( addr )
        0 swap !
    else 2drop then
;

: sgn ( n -- 1|0|-1 )
    dup if
        0> if 1 else -1 then
    then
;
