[IFUNDEF] cell-
: cell- [ 1 cells ] literal - ;
[THEN]

[IFUNDEF] sgn
: sgn ( n -- 1|0|-1 )
    dup if
        0> if 1 else -1 then
    then
;
[THEN]

[IFUNDEF] -!
: -! ( n addr -- )
    swap negate swap +!
;
[THEN]
