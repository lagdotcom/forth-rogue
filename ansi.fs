$1b constant ansi-escape

: ansi-bg-256 ( colour -- )
    <#
        'm' hold
        s>d #s
        ';' hold
        '5' hold
        ';' hold
        '8' hold
        '4' hold
        '[' hold
        ansi-escape hold
    #> type
;

: ansi-fg-256 ( colour -- )
    <#
        'm' hold
        s>d #s
        ';' hold
        '5' hold
        ';' hold
        '8' hold
        '3' hold
        '[' hold
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
0 constant black
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
