: inventory! { _inventory _cap _items -- }
    _cap _inventory inventory-capacity !
    _items _inventory inventory-items !
;

: add-inventory { _en _cap -- }
    _en ['] entity-inventory inventory% add-component
    _cap                            ( inv cap )
    dup cells allocate throw        ( inv cap items )
    dup _cap cells 0 fill           ( inv cap items )
    inventory!
;

:noname ( addr -- )
    dup @ ?dup-if
        inventory-items maybe-free
        maybe-free
    else drop then
; is maybe-free-inventory
