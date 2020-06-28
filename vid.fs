form * constant vidbuf-size

variable vidbuf-attr    vidbuf-size cells allot
variable vidbuf-ch      vidbuf-size allot
variable vidbuf-dirty   vidbuf-size allot

: screen-offset ( x y -- offset )
    cols * +
;

: vidbuf-attr-offset ( offset -- addr )
    cells vidbuf-attr +
;

: vidbuf-ch-offset ( offset -- addr )
    chars vidbuf-ch +
;

: vidbuf-dirty-offset ( offset -- addr )
    chars vidbuf-dirty +
;

: plot ( ch x y attr -- )
    \ TODO: rearrange arguments?
    >r                      ( ch x y )
    screen-offset           ( ch offset )

    dup vidbuf-attr-offset  ( ch offset addr )
    @ r@ <> if              ( ch offset )
        r>                  ( ch offset attr )
        over                ( ch offset attr offset )
        vidbuf-attr-offset  ( ch offset attr addr )
        !                   ( ch offset )
        true over           ( ch offset true offset )
        vidbuf-dirty-offset c!
    else
        rdrop
    then                    ( ch offset )

    over                    ( ch offset ch )
    over vidbuf-ch-offset   ( ch offset ch addr )
    c@ <> if                ( ch offset )
        tuck                ( offset ch offset )
        vidbuf-ch-offset    ( offset ch addr )
        c!                  ( offset )
        true swap           ( true offset )
        vidbuf-dirty-offset c!
    else
        2drop
    then
;

: present-offset ( offset -- )
    dup cols /mod at-xy
    dup vidbuf-attr-offset @ attr!
        vidbuf-ch-offset c@ emit
;

: present ( -- )
    vidbuf-size 0 do
        vidbuf-dirty i + c@ if
            false i vidbuf-dirty-offset c!
            i present-offset
        then
    loop
;

: vid-clear ( -- )
    vidbuf-attr     vidbuf-size cells 0 fill
    vidbuf-ch       vidbuf-size chars bl fill
    vidbuf-dirty    vidbuf-size chars true fill
;
