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

: free-inventory-contents { _inventory -- }
    _inventory inventory-items @
    _inventory inventory-capacity @ 0 ?do
        dup @ ?dup-if                               ( items item? )
            free-entity
            0 over !
        then cell+
    loop drop
;

:noname ( addr -- )
    dup @ ?dup-if
        dup free-inventory-contents
        inventory-items maybe-free
        maybe-free
    else drop then
; is maybe-free-inventory

: add-to-inventory { _en _item -- slot true | false }
    _en entity-inventory @ ?dup-if          ( inv )
        dup inventory-items @               ( inv items )
        swap inventory-capacity @           ( items cap )
        0 ?do                               ( items )
            dup @ if cell+ else             ( items )
                _item swap !
                i true unloop exit
            then
        loop drop
    then false
;
