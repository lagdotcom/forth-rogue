256 constant msg-buf-size
here constant msg-buf msg-buf-size allot

: <m ( -- str u )
    msg-buf 0
;

: m> ( str u -- c-addr )
    dup dup char+ allocate throw        ( str u u mem )
    dup >r !                            ( str u )
    r@ char+ swap cmove r>
;

: m|str { buf buf-count str str-count -- buf buf-count }
    str buf buf-count + str-count cmove
    buf buf-count str-count +
;

: m" ( str u "message" -- str u )
    [char] " parse postpone sliteral postpone m|str
; immediate

: m|num ( str u n -- str u )
    s>d <# #s #> m|str
;

: m|name ( str u entity -- str u )
    dup entity-name @
    swap entity-name-len @
    m|str
;
