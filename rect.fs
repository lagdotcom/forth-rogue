struct
    char% field rect-x1
    char% field rect-y1
    char% field rect-x2
    char% field rect-y2
end-struct rect%
rect% nip constant rect-size

: rect, ( y2 x2 y1 x1 -- )
    c, c, c, c,
;

: rect-convert { x y w h -- x1 y1 x2 y2 )
    x
    y
    x w +
    y h +
;

: rect-xywh, ( x y w h -- )
    rect-convert rect,
;

: rect@ { rect -- x1 y1 x2 y2 )
    rect rect-x1 c@
    rect rect-y1 c@
    rect rect-x2 c@
    rect rect-y2 c@
;

: rect! { rect x1 y1 x2 y2 -- }
    x1 rect rect-x1 c!
    y1 rect rect-y1 c!
    x2 rect rect-x2 c!
    y2 rect rect-y2 c!
;

: rect-centre { x1 y1 x2 y2 -- x y )
    x1 x2 + 2 /
    y1 y2 + 2 /
;

: rect-intersects { ax1 ay1 ax2 ay2 bx1 by1 bx2 by2 -- flag )
    ax1 bx2 <=
    ax2 bx1 >= and
    ay1 by2 <= and
    ay2 by1 >= and
;
