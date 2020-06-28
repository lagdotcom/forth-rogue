variable haltgame
variable player-x
variable player-y

27 constant k-esc

: draw-player ( -- )
    <A White >FG A> attr!
    player-x @ player-y @ at-xy '@' emit
;

: clear-player ( -- )
    player-x @ player-y @ at-xy bl emit
;

: move-player ( mx my -- )
    clear-player
    player-y +!
    player-x +!
;

: process-input ( -- )
    ekey
    case
        k-esc   of haltgame on endof
        'q'     of haltgame on endof

        k-up    of  0 -1 move-player endof
        k-right of  1  0 move-player endof
        k-down  of  0  1 move-player endof
        k-left  of -1  0 move-player endof
    endcase
;

: mainloop ( -- )
    haltgame off
    begin
        draw-player
        process-input
    haltgame @ until
;

page

cols 2 / player-x !
rows 2 / player-y !

mainloop
0 attr!
bye
