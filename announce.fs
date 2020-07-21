:noname ( s-addr -- )
    \ copy to debug log
    dup count 1 /string logwriteln
    add-to-log
; constant 'message

: <message ( -- xt str u )
    'message <m
;

: message> ( xt str u -- )
    m> add-action
;

: announce-entity-died { _en -- }
<message _en player = if red else orange then memit
    _en mname
    m"  dies!"
message> ;

: announce-attack-damage { _damage _attacker _victim -- }
<message white memit
    _attacker mname
    m"  attacks "
    _victim mname
    m"  for "
    _damage m.
    m"  damage."
message> ;

: announce-attack-failed { _attacker _victim -- }
<message white memit
    _attacker mname
    m"  attacks "
    _victim mname
    m"  but does no damage."
message> ;

: announce-player-got-item { _slot _en -- }
<message light-blue memit
    m" got ("
    _slot [char] a + memit
    m" ) "
    _en mname
    [char] . memit
message> ;

: announce-inventory-full ( -- )
<message yellow memit
    s" no room for more items." mtype
message> ;

: announce-get-failed ( -- )
<message yellow memit
    s" nothing to get." mtype
message> ;

: announce-unusable-item { _en -- }
<message yellow memit
    s" cannot use " mtype
    _en mname
message> ;

: announce-healed { _en -- }
<message
_en player = if
    green memit
    s" you feel better!" mtype
else
    red memit
    _en mname
    s"  looks better!" mtype
then message> ;

: announce-cannot-heal-more { _en -- }
_en player = if
<message
    yellow memit
    s" you're already healthy" mtype
message>
then ;

: announce-dropped-item { _en -- }
<message yellow memit
    s" dropped: " mtype
    _en mname
message> ;
