: item! { _item _flags _use -- }
    _flags _item item-flags !
    _use _item item-use-fn !
;

: add-item { _en _flags _use -- }
    _en ['] entity-item item% add-component
    _flags _use item!
;
