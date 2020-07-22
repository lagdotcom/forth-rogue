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

    _en distance-from-entity dup lightning-target-distance <= if
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

: item-lightning { _en _damage _range -- flag }
    _en _range get-lightning-target ?dup-if
        dup _damage announce-lightning-damage
        _damage damage
        true
    else
        announce-no-nearby-target
        false
    then
;

0 value fireball-damage
0 value fireball-radius
:noname { _en -- }
    _en entity-fighter @ 0= if exit then
    _en distance-from-entity fireball-radius <= if
        _en fireball-damage announce-fire-damage
        _en fireball-damage damage
    then
; constant 'apply-fireball

: item-fireball { _en _x _y _damage _radius -- flag }
    _x _y is-in-fov 0= if
        announce-must-target-in-fov false exit
    then

    announce-fireball
    _x _y to distance-from-y to distance-from-x
    _damage to fireball-damage
    _radius to fireball-radius
    'apply-fireball for-each-entity true
;

: item-confusion { _en _x _y -- flag }
    _x _y is-in-fov 0= if
        announce-must-target-in-fov false exit
    then

    _x _y get-blocker ?dup-if
        \ TODO
        false
    else
        announce-no-target false
    then
;
