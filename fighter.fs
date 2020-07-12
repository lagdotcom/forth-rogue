: fighter! { fighter max-hp hp defense power -- }
    max-hp fighter fighter-max-hp !
    hp fighter fighter-hp !
    defense fighter fighter-defense !
    power fighter fighter-power !
;

: add-fighter { entity hp defense power -- }
    entity ['] entity-fighter fighter% add-component
    hp hp defense power fighter!
;

: damage { entity amount -- }
    amount negate entity entity-fighter @ fighter-hp +!

    entity entity-fighter @ fighter-hp @ 1 < if
        'entity-died entity add-action
    then
;

: attack { attacker victim -- }
    attacker entity-fighter @ fighter-power @       ( power )
    victim entity-fighter @ fighter-defense @ -     ( damage )

    dup 0> if
        >r
        <message
            attacker        m|name
            m"  attacks "
            victim          m|name
            m"  for "
            r@              m|num
            m"  damage."
        message>

        victim r> damage
    else
        drop
        <message
            attacker                    m|name
            m"  attacks "
            victim                      m|name
            m"  but does no damage."
        message>
    then
;
