form 1- * constant vidbuf-size

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

: plot-attr { x y attr -- }
    attr x y screen-offset dup  ( attr offset offset )

    vidbuf-attr-offset dup @    ( attr offset addr old-attr )
    attr <> if
        true rot                ( attr addr true offset )
        vidbuf-dirty-offset c!  ( attr addr )
        !
    else
        2drop drop
    then
;

: plot-char { x y ch -- }
    ch 0= if exit then
    ch x y screen-offset dup    ( ch offset offset )

    vidbuf-ch-offset dup c@     ( ch offset addr old-ch )
    ch <> if
        true rot                ( ch addr true offset )
        vidbuf-dirty-offset c!  ( ch addr )
        c!
    else
        2drop drop
    then
;

: plot { ch x y attr -- }
    x y attr plot-attr
    x y ch plot-char
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
