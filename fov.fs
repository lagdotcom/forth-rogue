: init-fov ( -- )
    fov-map map-tiles false fill
;

: fov-offset ( x y -- offset )
    map-width * fov-map + +
;

: is-in-fov ( x y -- flag )
    fov-offset c@
;

0 value ray-minx
0 value ray-miny
0 value ray-maxx
0 value ray-maxy
0 value ray-rad2
0 value ray-dx
0 value ray-dy
0 value ray-sx
0 value ray-sy
0 value ray-x
0 value ray-y
0 value ray-err
: cast-ray { _x0 _y0 _x1 _y1 -- }
    _x0 to ray-x
    _y0 to ray-y
    _x1 _x0 - abs to ray-dx
    _x0 _x1 < if 1 else -1 then to ray-sx
    _y1 _y0 - abs negate to ray-dy
    _y0 _y1 < if 1 else -1 then to ray-sy
    ray-dx ray-dy + to ray-err

    begin
        ray-x _x0 - abs dup *
        ray-y _y0 - abs dup * +     ( dx^2+dy^2 )
        ray-rad2 <=
        ray-x ray-y map-contains and
    while
        true ray-x ray-y fov-offset c!
        ray-x ray-y map-offset c@
        TILE_BLOCKED and if
            exit
        then

        ray-x _x1 = ray-y _y1 = and if
            exit
        then

        ray-err 2 *
        dup ray-dy >= if
            ray-err ray-dy + to ray-err
            ray-x ray-sx + to ray-x
        then
        ray-dx <= if
            ray-err ray-dx + to ray-err
            ray-y ray-sy + to ray-y
        then
    repeat
;

: do-raycasting-fov { _x _y _radius -- }
    0 _x _radius - max to ray-minx
    0 _y _radius - max to ray-miny
    map-width 1- _x _radius + min to ray-maxx
    map-height 1- _y _radius + min to ray-maxy
    _radius _radius * to ray-rad2

    ray-maxx ray-minx ?do
        _x _y i ray-miny cast-ray
        _x _y i ray-maxy cast-ray
    loop
    ray-maxy ray-miny ?do
        _x _y ray-minx i cast-ray
        _x _y ray-maxx i cast-ray
    loop
;

: recompute-fov ( -- )
    init-fov
    player entity-xy@
    fov-radius              ( x y radius )
    do-raycasting-fov
;
