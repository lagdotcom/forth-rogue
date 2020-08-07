0 value save-file
defer save-entity

: save-ln ( str u -- )
    save-file write-line throw
;

: save-type ( str u -- )
    save-file write-file throw
;

: save. ( x -- )
    s>d <# bl hold #s #> save-type
;

: save-char ( ch -- )
    save-file emit-file throw
;

: save-sp ( -- )
    bl save-file emit-file throw
;

: save-indent ( -- )
    save-sp save-sp
;

: save-fighter { _fighter _en -- }
    save-indent
    s" dup " save-type
    _fighter fighter-max-hp @ save.
    _fighter fighter-defense @ save.
    _fighter fighter-power @ save.
    _fighter fighter-xp @ save.
    s" add-fighter" save-ln

    _fighter fighter-max-hp @
    _fighter fighter-hp @ <> if
        save-indent
        _fighter fighter-hp @ save.
        s" over " save-type
        s" entity-fighter @ fighter-hp !" save-ln
    then
;

: save-ai { _ai _en -- }
    _ai ai-fn @ 'confused-ai = if
        \ save the old one first, so there's something to switch to!
        _ai ai-data0 @ _en recurse
    then

    save-indent
    s" dup " save-type

    _ai ai-fn @ case
        'basic-ai of s" apply-basic-ai" save-ln endof
        'confused-ai of
            _ai ai-data1 @ save.
            s" apply-confused-ai" save-ln
        endof
    endcase
;

: save-inventory { _inventory _en -- }
    save-indent
    s" dup " save-type

    _inventory inventory-capacity @ save.
    s" add-inventory" save-type

    _inventory inventory-items @
    _inventory inventory-capacity @ 0 ?do
        dup @ ?dup-if
            save-entity save-indent save-indent
            s" over " save-type
            s" entity-inventory @ inventory-items @ " save-type
            i save. s" cells + !" save-type
        then cell+
    loop drop

    newline save-type
;

: save-item { _item _en -- }
    save-indent
    s" dup " save-type

    _item item-use @ lookup-item-use save-type
    s"  add-item" save-ln
;

: save-stairs { _stairs _en -- }
    save-indent
    s" dup " save-type
    _stairs stairs-floor @ save.
    s" add-stairs" save-ln
;

: save-level { _level _en -- }
    save-indent
    s" dup " save-type
    _level level-current @ save.
    _level level-xp @ save.
    _level level-base @ save.
    _level level-factor @ save.
    s" add-level" save-ln
;

:noname { _en -- }
    newline save-type

    _en entity-ch @ save.
    _en entity-x @ save.
    _en entity-y @ save.
    _en entity-fg @ save.
    [char] c save-char [char] " save-char save-sp
        _en entity-name@ save-type
    [char] " save-char save-sp
    _en entity-layer @ save.
    _en entity-flags @ save.

    s" alloc-entity" save-ln

    _en entity-fighter @ ?dup-if _en save-fighter then
    _en entity-ai @ ?dup-if _en save-ai then
    _en entity-inventory @ ?dup-if _en save-inventory then
    _en entity-item @ ?dup-if _en save-item then
    _en entity-stairs @ ?dup-if _en save-stairs then
    _en entity-level @ ?dup-if _en save-level then
; is save-entity

: save-full-entity ( en -- )
    dup save-entity
    player = if s" dup to player " save-type then
    s" add-entity" save-ln
;

: save-game ( filename u -- )
    w/o create-file throw to save-file
    s" :noname ansi-reset" save-ln
    map-seed save. s" seed !" save-ln
    dungeon-level save. s" to dungeon-level" save-ln
    s" player " save-type
        map-min save.
        map-max save.
        map-rooms save.
        map-monsters save.
        map-items save.
    s" generate-map free-all-entities" save-ln

    ['] save-full-entity for-each-entity
    s" ; execute" save-ln
    save-file close-file throw
;
