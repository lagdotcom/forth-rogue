26 constant menu-size
menu-size cells allocate throw constant menu-buf
menu-buf menu-size cells 0 fill
0 value menu-items

: clear-menu ( -- )
    menu-buf menu-items 0 ?do
        dup maybe-free
        cell+
    loop drop

    0 to menu-items
;

: add-menu-item ( c-addr index -- )
    \ TODO: check menu isn't too big?
    dup menu-items max 1+ to menu-items
    cells menu-buf + !
;

0 variable menu-x
0 variable menu-y
: show-menu { _str _str-len -- }
    cols _str-len - 2 / menu-x !
    rows menu-items 1+ - 2 / menu-y !
    menu-x @ menu-y @ 1- light-grey black _str _str-len plot-str

    menu-buf menu-items 0 ?do
        dup @ ?dup-if
               menu-x @ menu-y @       black _str-len plot-spaces
            >r menu-x @ menu-y @ white black r> count plot-str
            1 menu-y +!
        then cell+
    loop drop
;

: cleanup ( -- )
    s" - clearing menu" logwriteln
    clear-menu

    s" - freeing menu" logwriteln
    menu-buf free throw
cleanup ;
