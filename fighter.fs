: fighter! { fighter max-hp hp defense power -- }
    max-hp fighter fighter-max-hp !
    hp fighter fighter-hp !
    defense fighter fighter-defense !
    power fighter fighter-power !
;

: add-fighter { entity hp defense power -- }
    entity ['] entity-fighter fighter-size add-component
    hp hp defense power fighter!
;
