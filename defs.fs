struct
    cell% field entity-ch
    cell% field entity-x
    cell% field entity-y
    cell% field entity-fg
    cell% field entity-name-len
    cell% field entity-name
    cell% field entity-flags
    cell% field entity-fighter
    cell% field entity-ai
end-struct entity%
entity% %size constant entity-size

struct
    char% field rect-x1
    char% field rect-y1
    char% field rect-x2
    char% field rect-y2
end-struct rect%
rect% %size constant rect-size

struct
    cell% field fighter-max-hp
    cell% field fighter-hp
    cell% field fighter-defense
    cell% field fighter-power
end-struct fighter%

struct
    cell% field ai-fn           ( entity -- )
end-struct ai%

defer clear-entity
