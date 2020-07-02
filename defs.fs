struct
    cell% field entity-ch
    cell% field entity-x
    cell% field entity-y
    cell% field entity-fg
    cell% field entity-name
    cell% field entity-name-len
    cell% field entity-flags
end-struct entity%
entity% nip constant entity-size

struct
    char% field rect-x1
    char% field rect-y1
    char% field rect-x2
    char% field rect-y2
end-struct rect%
rect% nip constant rect-size
