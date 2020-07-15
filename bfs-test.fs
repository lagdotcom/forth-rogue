: cleanup ( -- ) ;
: logwriteln ( str u -- ) 2drop ;

true constant debug-bfs
include queue.fs
include bfs.fs

10 constant map-width
10 constant map-height

map-width map-height make-nodemap constant map

: make-test-map ( -- )
    1 1 1 1 1 1 1 1 1 1
    1 0 0 1 0 0 0 0 0 1
    1 0 0 1 0 0 0 0 0 1
    1 0 0 1 0 0 0 0 0 1
    1 0 0 1 0 0 1 0 0 1
    1 0 0 1 0 0 1 0 0 1
    1 0 0 0 0 0 1 0 0 1
    1 0 0 0 0 0 1 0 0 1
    1 0 0 0 0 0 1 0 0 1
    1 1 1 1 1 1 1 1 1 1
    map-height 0 ?do
        map-width 0 ?do
            if
                i j map at-nodemap set-blocked
            then
        loop
    loop
;

make-test-map
map-width map-height map show-nodemap

hex
1 1 8 8 map bfs
.s

cleanup
bye
