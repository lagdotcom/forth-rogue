: item! { _item _use -- }
    _use _item item-use !
;

: add-item { _en _use -- }
    _en ['] entity-item item% add-component
    _use item!
;

: item-heal { _en _amount -- _flag }
    \ TODO
    true
;
