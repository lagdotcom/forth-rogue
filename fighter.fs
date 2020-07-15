: fighter! { _fighter _max-hp _hp _defense _power -- }
    _max-hp _fighter fighter-max-hp !
    _hp _fighter fighter-hp !
    _defense _fighter fighter-defense !
    _power _fighter fighter-power !
;

: add-fighter { _en _hp _defense _power -- }
    _en ['] entity-fighter fighter% add-component
    _hp _hp _defense _power fighter!
;

: damage { _en _amount -- }
    _amount negate _en entity-fighter @ fighter-hp +!

    _en entity-fighter @ fighter-hp @ 1 < if
        'entity-died _en add-action
    then
;

: attack { _attacker _victim -- }
    _attacker entity-fighter @ fighter-power @       ( power )
    _victim entity-fighter @ fighter-defense @ -     ( damage )

    dup 0> if
        >r
        <message
            _attacker       mname
            m"  attacks "
            _victim         mname
            m"  for "
            r@              m.
            m"  damage."
        message>

        _victim r> damage
    else
        drop
        <message
            _attacker       mname
            m"  attacks "
            _victim         mname
            m"  but does no damage."
        message>
    then
;
