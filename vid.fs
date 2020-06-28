form * constant buffer-size

variable buffer-attr    buffer-size cells allot
variable buffer-ch      buffer-size allot
variable buffer-dirty   buffer-size allot

: screen-offset ( x y -- offset )
    cols * +
;

: buffer-attr-offset ( offset -- addr )
    cells buffer-attr +
;

: buffer-ch-offset ( offset -- addr )
    chars buffer-ch +
;

: buffer-dirty-offset ( offset -- addr )
    chars buffer-dirty +
;

: plot ( ch x y attr -- )
    \ TODO: rearrange arguments?
    >r                      ( ch x y )
    screen-offset           ( ch offset )

    dup buffer-attr-offset  ( ch offset addr )
    @ r@ <> if              ( ch offset )
        r>                  ( ch offset attr )
        over                ( ch offset attr offset )
        buffer-attr-offset  ( ch offset attr addr )
        !                   ( ch offset )
        true over           ( ch offset true offset )
        buffer-dirty-offset c!
    else
        rdrop
    then                    ( ch offset )

    over                    ( ch offset ch )
    over buffer-ch-offset   ( ch offset ch addr )
    c@ <> if                ( ch offset )
        tuck                ( offset ch offset )
        buffer-ch-offset    ( offset ch addr )
        c!                  ( offset )
        true swap           ( true offset )
        buffer-dirty-offset c!
    else
        2drop
    then
;

: present-offset ( offset -- )
    dup cols /mod at-xy
    dup buffer-attr-offset @ attr!
        buffer-ch-offset c@ emit
;

: present ( -- )
    buffer-size 0 do
        buffer-dirty i + c@ if
            false i buffer-dirty-offset c!
            i present-offset
        then
    loop
;

: vid-clear ( -- )
    buffer-attr     buffer-size cells 0 fill
    buffer-ch       buffer-size chars bl fill
    buffer-dirty    buffer-size chars true fill
;
