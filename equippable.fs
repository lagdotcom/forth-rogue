: equippable! { _equippable _slot _power _defense _hp -- }
    _slot _equippable equippable-slot !
    _power _equippable equippable-power-bonus !
    _defense _equippable equippable-defense-bonus !
    _hp _equippable equippable-hp-bonus !
;

: add-equippable { _en _slot _power _defense _hp -- }
    _en ['] entity-equippable equippable% add-component
    _slot _power _defense _hp equippable!
;
