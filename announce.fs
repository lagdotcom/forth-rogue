:noname ( s-addr -- )
    \ copy to debug log
    dup count logwriteln
    add-to-log
; constant 'message

: <message ( -- xt str u )
    'message <m
;

: message> ( xt str u -- )
    m> add-action
;

: announce-entity-died { _en -- }
<message
    _en mname
    m"  dies!"
message> ;

: announce-attack-damage { _damage _attacker _victim -- }
<message
    _attacker mname
    m"  attacks "
    _victim mname
    m"  for "
    _damage m.
    m"  damage."
message> ;

: announce-attack-failed { _attacker _victim -- }
<message
    _attacker mname
    m"  attacks "
    _victim mname
    m"  but does no damage."
message> ;

: announce-player-got-item { _slot _en -- }
<message
    m" got ("
    _slot [char] a + memit
    m" ) "
    _en mname
    [char] . memit
message> ;

: announce-inventory-full ( -- )
<message
    s" no room for more items." mtype
message> ;

: announce-get-failed ( -- )
<message
    s" nothing to get." mtype
message> ;
