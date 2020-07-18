\ Breath-first search.

\ procedure BFS(G, root) is
\     let Q be a queue
\     label root as discovered
\     Q.enqueue(root)
\     while Q is not empty do
\         v := Q.dequeue()
\         if v is the goal then
\             return v
\         for all edges from v to w in G.adjacentEdges(v) do
\             if w is not labeled as discovered then
\                 label w as discovered
\                 w.parent := v
\                 Q.enqueue(w)

struct
    cell% field node-x
    cell% field node-y
    cell% field node-blocked
    cell% field node-parent
    cell% field node-discovered
end-struct node%
node% %size constant node-size

struct
    cell% field nodemap-w
    cell% field nodemap-h
end-struct nodemap%
nodemap% %size constant nodemap-size

: set-blocked ( node -- )
    node-blocked on
;

: set-discovered ( node -- )
    node-discovered on
;

: is-discovered ( node -- )
    node-discovered @
;

: set-parent ( parent node -- )
    node-parent !
;

: node-xy@ ( node -- x y )
    dup node-x @
    swap node-y @
;

: nodes-equal { _n1 _n2 -- flag }
    _n1 node-xy@
    _n2 node-xy@
    d=
;

: setup-node { _n _x _y -- }
    _x _n node-x !
    _y _n node-y !
    0 _n node-parent !
    0 _n node-blocked !
    0 _n node-discovered !
;

: nodemap-cells-size ( w h -- size )
    node-size * *
;

: nodemap-total-size ( w h -- size )
    nodemap-cells-size nodemap-size +
;

: nodemap-contains { _x _y _nodemap -- flag }
    _x 0 _nodemap nodemap-w @ within 0= if
        drop false exit
    then
    _y 0 _nodemap nodemap-h @ within 0= if
        false exit
    else true then
;

: at-nodemap { _x _y _nodemap -- node|0 }
    _x _y _nodemap nodemap-contains if
        _nodemap nodemap-size +     ( nodemap-nodes )
        _y _nodemap nodemap-w @ *   ( nodemap-nodes y*w )
        _x +                        ( nodemap-nodes y*w+x )
        node-size * +
    else false then
;

: nodemap-blocked ( x y nodemap -- flag )
    at-nodemap ?dup-if
        node-blocked @
    else true then
;

: setup-nodemap-nodes { _nodemap -- }
    _nodemap nodemap-h @ 0 ?do
        _nodemap nodemap-w @ 0 ?do
            i j _nodemap at-nodemap i j setup-node
        loop
    loop
;

: setup-nodemap { _nodemap _w _h -- }
    _w _nodemap nodemap-w !
    _h _nodemap nodemap-h !
    _nodemap setup-nodemap-nodes
;

: make-nodemap { _w _h -- nodemap }
    _w _h nodemap-total-size allocate throw
    dup _w _h setup-nodemap
;

: free-nodemap ( nodemap -- )
    free throw
;

: show-nodemap { _w _h _nodemap -- }
    _h 0 ?do
        _w 0 ?do
            i j _nodemap at-nodemap node-blocked @
            if [char] # else bl then emit
        loop
        cr
    loop
;

: test-node-edge { _x _y _mx _my _nodemap -- edge 1 | 0 }
    _x _mx +
    _y _my +
    2dup _nodemap nodemap-blocked if
        2drop false
    else
        _nodemap at-nodemap true
    then
;

\ TODO: refactor
: get-node-edges { _n _nodemap -- edge... count }
    0 >r
    _n node-xy@ 1 0 _nodemap test-node-edge if
        r> 1+ >r
    then
    _n node-xy@ -1 0 _nodemap test-node-edge if
        r> 1+ >r
    then
    _n node-xy@ 0 1 _nodemap test-node-edge if
        r> 1+ >r
    then
    _n node-xy@ 0 -1 _nodemap test-node-edge if
        r> 1+ >r
    then
    r>
;

2000 queue: bfs-queue
0 value root-node
0 value goal-node
0 value v-node
0 value w-edge
: bfs-do { _nodemap -- flag }
    bfs-queue clear-queue
    root-node set-discovered
    root-node bfs-queue enqueue
    begin bfs-queue queue-is-empty 0= while
        bfs-queue dequeue to v-node
        v-node goal-node nodes-equal if
            v-node true exit
        then

        v-node _nodemap get-node-edges 0 ?do
            to w-edge
            w-edge is-discovered 0= if
                w-edge set-discovered
                v-node w-edge set-parent
                w-edge bfs-queue enqueue
            then
        loop
    repeat
    false
;


: bfs { _sx _sy _dx _dy _nodemap -- x y true | false }
    _dx _dy _nodemap at-nodemap to goal-node
    _sx _sy _nodemap at-nodemap to root-node

    _nodemap bfs-do if       ( goal )
        begin dup node-parent @ root-node nodes-equal 0= while
            [ [IFDEF] debug-bfs ]
                dup node-xy@ swap . . cr
            [ [THEN] ]
            node-parent @
        repeat
        node-xy@ true
    else false then
;

: cleanup ( -- )
    s" - freeing BFS queue" logwriteln
    bfs-queue free-queue
cleanup ;
