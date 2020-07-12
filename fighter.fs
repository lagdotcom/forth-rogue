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
;

: attack { attacker victim -- }
    attacker entity-fighter @ fighter-power @       ( power )
    victim entity-fighter @ fighter-defense @ -     ( damage )
    
    dup 0> if
        dup victim swap damage                      ( damage )
        attacker entity.name ." attacks "
        victim entity.name ." for "
        . ." damage." cr
    else
        drop
        attacker entity.name ." attacks "
        victim entity.name ." but does no damage." cr
    then
;
