: equipment! { _equipment _mainhand _offhand -- }
    _mainhand _equipment equipment-main-hand !
    _offhand _equipment equipment-off-hand !
;

: add-equipment { _en _mainhand _offhand -- }
    _en ['] entity-equipment equipment% add-component
    _mainhand _offhand equipment!
;

: get-equipped { _en _slot -- enitem|0 }
    _en entity-equipment @ if
        _slot _en entity-equipment @ + @
    else 0 then
;

: equipment-slot@ { _en -- _slot }
    _en entity-equippable @ equippable-slot @
;

: change-equipment { _val _slot _en -- }
    _val _slot _en entity-equipment @ + !
;

: unequip { _en _enitem -- }
    0 _enitem equipment-slot@ _en change-equipment
    _en _enitem announce-unequipped-item
;

: equip { _en _enitem -- }
    _enitem _enitem equipment-slot@ _en change-equipment
    _en _enitem announce-equipped-item
;

: is-equipped { _en _enitem -- flag }
    _enitem entity-equippable @ if
        _en _enitem equipment-slot@ get-equipped _enitem =
    else 0 then
;

: toggle-equip { _en _enitem -- }
    _en _enitem is-equipped if
        _en _enitem unequip exit
    then

    _en _enitem equipment-slot@ get-equipped ?dup-if
        _en swap unequip
    then
    _en _enitem equip
;

: get-equip-bonus { _en _bonus -- number }
    0
    equipment-slots 0 ?do
        _en i cells get-equipped ?dup-if
            entity-equippable @ _bonus +
            @ +
        then
    loop
;

: get-stat { _en _bonus -- number }
    _en entity-fighter @ _bonus + @
    _en entity-equipment @ if
        _en _bonus get-equip-bonus +
    then
;

:noname ( en -- number )
    BONUS_POWER get-stat
; is get-power

:noname ( en -- number )
    BONUS_DEFENSE get-stat
; is get-defense

:noname ( en -- number )
    BONUS_HP get-stat
; is get-max-hp
