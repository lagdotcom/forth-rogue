: cleanup ( -- ) ;

include bfs.fs

10 constant width
10 constant height

width height make-nodemap constant map

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
    height 0 ?do
        width 0 ?do
            if
                i j map at-nodemap set-blocked
            then
        loop
    loop
;

make-test-map
width height map show-nodemap

hex
1 1 8 8 map bfs
.s

cleanup
bye
