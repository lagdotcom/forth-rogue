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

: nodes-equal { n1 n2 -- flag }
    n1 node-x @ n1 node-y @
    n2 node-x @ n2 node-y @
    d=
;

: setup-node { node x y -- }
    x node node-x !
    y node node-y !
    0 node node-parent !
    0 node node-blocked !
    0 node node-discovered !
;

: nodemap-cells-size ( w h -- size )
    node-size * *
;

: nodemap-total-size ( w h -- size )
    nodemap-cells-size nodemap-size +
;

: nodemap-contains { x y nodemap -- flag }
    x 0 nodemap nodemap-w @ within 0= if
        drop false exit
    then
    y 0 nodemap nodemap-h @ within 0= if
        false exit
    else true then
;

: at-nodemap { x y nodemap -- node|0 }
    x y nodemap nodemap-contains if
        nodemap nodemap-size +      ( nodemap-nodes )
        y nodemap nodemap-w @ *     ( nodemap-nodes y*w )
        x +                         ( nodemap-nodes y*w+x )
        node-size * +
    else false then
;

: nodemap-blocked { x y nodemap -- flag }
    x y nodemap at-nodemap dup if
        node-blocked @
    else 0= then
;

: setup-nodemap-nodes { nodemap -- }
    nodemap nodemap-h @ 0 ?do
        nodemap nodemap-w @ 0 ?do
            i j nodemap at-nodemap i j setup-node
        loop
    loop
;

: setup-nodemap { nodemap w h -- }
    w nodemap nodemap-w !
    h nodemap nodemap-h !
    nodemap setup-nodemap-nodes
;

: make-nodemap { w h -- nodemap }
    w h nodemap-total-size allocate throw
    dup w h setup-nodemap
;

: free-nodemap ( nodemap -- )
    free throw
;

: show-nodemap { width height nodemap -- }
    height 0 ?do
        width 0 ?do
            i j nodemap at-nodemap node-blocked @
            if '#' else bl then emit
        loop
        cr
    loop
;

: test-node-edge { x y mx my nodemap -- edge 1 | 0 }
    x mx +
    y my +
    2dup nodemap nodemap-blocked if
        2drop false
    else
        nodemap at-nodemap true
    then
;

: node-xy@ ( node -- x y )
    dup node-x @
    swap node-y @
;

\ TODO: refactor
: get-node-edges { node nodemap -- edge... count }
    0 >r
    node node-xy@ 1 0 nodemap test-node-edge if
        r> 1+ >r
    then
    node node-xy@ -1 0 nodemap test-node-edge if
        r> 1+ >r
    then
    node node-xy@ 0 1 nodemap test-node-edge if
        r> 1+ >r
    then
    node node-xy@ 0 -1 nodemap test-node-edge if
        r> 1+ >r
    then
    r>
;

2000 queue: bfs-queue
0 value root-node
0 value goal-node
0 value v-node
0 value w-edge
: bfs-do { nodemap -- flag }
    bfs-queue clear-queue
    root-node set-discovered
    root-node bfs-queue enqueue
    begin bfs-queue queue-is-empty 0= while
        bfs-queue dequeue to v-node
        v-node goal-node nodes-equal if
            v-node true exit
        then

        v-node nodemap get-node-edges 0 ?do
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

false constant bfs-debug
: bfs { sx sy dx dy nodemap -- x y true | false }
    dx dy nodemap at-nodemap to goal-node
    sx sy nodemap at-nodemap to root-node

    nodemap bfs-do if       ( goal )
        begin dup node-parent @ root-node nodes-equal 0= while
            [ bfs-debug [IF] ]
                dup node-xy@ swap . . cr
            [ [ENDIF] ]
            node-parent @
        repeat
        node-xy@ true
    else false then
;

: cleanup ( -- )
    bfs-queue free-queue
cleanup ;
