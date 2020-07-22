: item! { _item _use -- }
    _use _item item-use !
;

: add-item { _en _use -- }
    _en ['] entity-item item% add-component
    _use item!
;

: item-heal { _en _amount -- flag }
    _en entity-hp@max@ < if
        _en _amount heal
        _en announce-healed
        true
    else
        _en announce-cannot-heal-more
        false
    then
;

0 value distance-from-x
0 value distance-from-y
: distance-from-entity { _en -- distance }
    distance-from-x distance-from-y _en entity-xy@ distance
;

0 value lightning-ignore
0 value lightning-target
0 value lightning-target-distance
:noname { _en -- }
    _en lightning-ignore = if exit then
    _en entity-fighter @ 0= if exit then
    _en entity-xy@ is-in-fov 0= if exit then

    _en distance-from-entity dup lightning-target-distance < if
        to lightning-target-distance
        _en to lightning-target
    else drop then
; constant 'get-lightning-target

: get-lightning-target { _en _range -- target }
    _en entity-xy@ to distance-from-y to distance-from-x
    _en to lightning-ignore
    0 to lightning-target
    1000 to lightning-target-distance
    'get-lightning-target for-each-entity
    lightning-target
;

: item-lightning { _en _amount _range -- flag }
    _en _range get-lightning-target ?dup-if
        dup _amount announce-lightning-damage
        _amount damage
        true
    else
        announce-no-target
        false
    then
;
