256 constant msg-buf-size
create msg-buf msg-buf-size allot

align create msg-log msg-log-size cells allot

: add-to-log ( s-addr -- )
    msg-log @ ?dup-if free throw then

    msg-log msg-log-size 1 ?do
        cell+
        dup dup @ swap cell- !
    loop

    !
;

: <m ( -- str u )
    msg-buf 0
;

: m> ( str u -- c-addr )
    dup dup char+ allocate throw        ( str u u mem )
    dup >r !                            ( str u )
    r@ char+ swap cmove r>
;

: mtype { _buf _buf-count _str _str-count -- buf buf-count }
    _str _buf _buf-count + _str-count cmove
    _buf _buf-count _str-count +
;

: m" ( str u "message" -- str u )
    [char] " parse postpone sliteral postpone mtype
; immediate

: m. ( str u n -- str u )
    s>d <# #s #> mtype
;
