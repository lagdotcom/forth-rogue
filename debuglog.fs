s" debug.log" w/o create-file throw constant log-file

: logtype ( str u -- )
    log-file write-file throw
;

: logemit ( ch -- )
    log-file emit-file throw
;

: logd. ( d -- )
    <# #s #> logtype
;

: log. ( n -- )
    s>d logd.
;

: <log ( -- )
    [char] [ logemit
    utime logd.
    [char] ] logemit
    bl logemit
;

: log> ( -- )
    newline logtype
;

: logwriteln ( str u -- )
    <log
    log-file write-line throw
;

: logclose ( -- )
    log-file close-file throw
;

[IFDEF] debug-allocations
: allocate { _size -- mem ior }
    <log
        s" ALLOC sz=" logtype
        _size log.
        _size allocate
        ?dup-if
            s"  ior=" logtype
            dup log.
        else
            s"  addr=" logtype
            dup hex log. decimal
            0
        then
    log>
;

: free { _mem -- ior }
    <log
        s" FREE addr=" logtype
        _mem hex log. decimal
        _mem free
        ?dup-if
            s"  ior=" logtype
            dup log.
        else
            s"  ok" logtype
            0
        then
    log>
;

: %alloc ( align size -- )
    nip allocate throw
;
[THEN]
