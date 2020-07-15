: rect, ( y2 x2 y1 x1 -- )
    c, c, c, c,
;

: rect-convert { _x _y _w _h -- x1 y1 x2 y2 }
    _x
    _y
    _x _w +
    _y _h +
;

: rect@ { _rect -- x1 y1 x2 y2 }
    _rect rect-x1 c@
    _rect rect-y1 c@
    _rect rect-x2 c@
    _rect rect-y2 c@
;

: rect! { _rect _x1 _y1 _x2 _y2 -- }
    _x1 _rect rect-x1 c!
    _y1 _rect rect-y1 c!
    _x2 _rect rect-x2 c!
    _y2 _rect rect-y2 c!
;

: rect-centre { _x1 _y1 _x2 _y2 -- x y }
    _x1 _x2 + 2 /
    _y1 _y2 + 2 /
;

: rect-intersects { _ax1 _ay1 _ax2 _ay2 _bx1 _by1 _bx2 _by2 -- flag }
    _ax1 _bx2 <=
    _ax2 _bx1 >= and
    _ay1 _by2 <= and
    _ay2 _by1 >= and
;
