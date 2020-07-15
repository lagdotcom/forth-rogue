[IFUNDEF] cell-
: cell- [ 1 cells ] literal - ;
[ENDIF]

[IFUNDEF] sgn
: sgn ( n -- 1|0|-1 )
    dup if
        0> if 1 else -1 then
    then
;
[ENDIF]
