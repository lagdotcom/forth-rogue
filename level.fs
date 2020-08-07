: level! { _level _current _xp _base _factor -- }
    _current _level level-current !
    _xp _level level-xp !
    _base _level level-base !
    _factor _level level-factor !
;

: add-level { _en _current _xp _base _factor -- }
    _en ['] entity-level level% add-component
    _current _xp _base _factor level!
;

: to-next-level { _en -- xp }
    _en entity-level @ level-current @
    _en entity-level @ level-factor @ *
    _en entity-level @ level-base @ +
;

:noname { _en _amount -- flag }
    _amount _en entity-level @ level-xp +!
    _en entity-level @ level-xp @
    _en to-next-level >= if
        _en to-next-level _en entity-level @ level-xp -!
        1 _en entity-level @ level-current +!
        true
    else false then
; is add-xp

:noname ( -- )
    clear-menu
    <m
        m" (c) Constitution (+20 HP, from "
        player entity-fighter @ fighter-max-hp @ m.
        [char] ) memit
    m> 0 add-menu-item
    <m
        m" (s) Strength (+1 attack, from "
        player entity-fighter @ fighter-power @ m.
        [char] ) memit
    m> 1 add-menu-item
    <m
        m" (a) Agility (+1 defense, from "
        player entity-fighter @ fighter-defense @ m.
        [char] ) memit
    m> 2 add-menu-item

    s" Choose level up bonus." show-menu present
    begin
        ekey ekey>char if case
            [char] c of
                20 player entity-fighter @ fighter-max-hp +!
                20 player entity-fighter @ fighter-hp +!
                announce-gained-con
            exit endof

            [char] s of
                1 player entity-fighter @ fighter-power +!
                announce-gained-str
            exit endof

            [char] a of
                1 player entity-fighter @ fighter-defense +!
                announce-gained-agi
            exit endof
        endcase else drop then
    again
; is choose-level-bonus
