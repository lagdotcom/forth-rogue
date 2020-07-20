$1b constant ansi-escape

: ansi-bg-256 ( colour -- )
    <#
        [char] m hold
        s>d #s
        [char] ; hold
        [char] 5 hold
        [char] ; hold
        [char] 8 hold
        [char] 4 hold
        [char] [ hold
        ansi-escape hold
    #> type
;

: ansi-fg-256 ( colour -- )
    <#
        [char] m hold
        s>d #s
        [char] ; hold
        [char] 5 hold
        [char] ; hold
        [char] 8 hold
        [char] 3 hold
        [char] [ hold
        ansi-escape hold
    #> type
;

: ansi-bold ( -- )
    ansi-escape emit
    ." [1m"
;

: ansi-underline ( -- )
    ansi-escape emit
    ." [4m"
;

: ansi-blink ( -- )
    ansi-escape emit
    ." [5m"
;

: ansi-reset ( -- )
    ansi-escape emit
    ." [m"
;

\ 256-bit colours
0 constant transparent
1 constant red
2 constant green
3 constant yellow
4 constant blue
5 constant magenta
6 constant cyan
7 constant light-grey
8 constant dark-grey
9 constant light-red
10 constant light-green
11 constant light-yellow
12 constant light-blue
13 constant light-magenta
14 constant light-cyan
15 constant white
16 constant black
92 constant violet
202 constant orange
