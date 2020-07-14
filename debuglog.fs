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
    '[' logemit
    utime logd.
    ']' logemit
    bl logemit
;

: log> ( -- )
    \ TODO: this seems unsafe a bit?
    10 logemit
;

: logwriteln ( str u -- )
    <log
    log-file write-line throw
;

: logclose ( -- )
    log-file close-file throw
;
