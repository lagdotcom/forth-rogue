: fighter! { _fighter _max-hp _hp _defense _power _xp -- }
    _max-hp _fighter fighter-max-hp !
    _hp _fighter fighter-hp !
    _defense _fighter fighter-defense !
    _power _fighter fighter-power !
    _xp _fighter fighter-xp !
;

: add-fighter { _en _hp _defense _power _xp -- }
    _en ['] entity-fighter fighter% add-component
    _hp _hp _defense _power _xp fighter!
;

:noname { _en -- }
    _en announce-entity-died

    _en clear-entity
    _en remove-entity

    \ make a corpse
    [char] % _en entity-xy@
    red
    <m m" remains of " _en mname m>
    LAYER_CORPSE
    ENTITY_NAME_ALLOC
    alloc-entity add-entity

    \ don't free the player, they have special considerations
    _en player <> if
        player _en entity-fighter @ fighter-xp @ dup announce-gained-xp add-xp if
            choose-level-bonus refresh-ui
        then

        _en free-entity
    then
; constant 'entity-died

: damage { _en _amount -- }
    _amount _en entity-fighter @ fighter-hp -!

    _en entity-fighter @ fighter-hp @ 1 < if
        'entity-died _en add-action
    then
;

: attack { _attacker _victim -- }
    _attacker entity-fighter @ fighter-power @       ( power )
    _victim entity-fighter @ fighter-defense @ -     ( damage )

    dup 0> if
        dup _attacker _victim announce-attack-damage
        _victim swap damage
    else
        drop
        _attacker _victim announce-attack-failed
    then
;

: entity-hp@max@ ( en -- hp max-hp )
    entity-fighter @ dup fighter-hp @ swap fighter-max-hp @
;

: heal { _en _amount -- }
    _en entity-hp@max@ swap _amount + min
    _en entity-fighter @ fighter-hp !
;
