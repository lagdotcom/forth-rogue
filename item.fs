: item! { _item _use -- }
    _use _item item-use !
;

: add-item { _en _use -- }
    _en ['] entity-item item% add-component
    _use item!
;

: item-heal { _en _amount -- _flag }
    _en entity-hp@max@ < if
        _en _amount heal
        _en announce-healed
        true
    else
        _en announce-cannot-heal-more
        false
    then
;
