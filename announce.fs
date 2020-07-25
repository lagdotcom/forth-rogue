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
    m" no room for more items."
message> ;

: announce-get-failed ( -- )
<message yellow memit
    m" nothing to get."
message> ;

: announce-unusable-item { _en -- }
<message yellow memit
    m" cannot use "
    _en mname
message> ;

: announce-healed { _en -- }
<message
_en player = if
    green memit
    m" you feel better!"
else
    red memit
    _en mname
    m"  looks better!"
then message> ;

: announce-cannot-heal-more { _en -- }
_en player = if
<message
    yellow memit
    m" you're already healthy"
message>
then ;

: announce-dropped-item { _en -- }
<message yellow memit
    m" dropped: "
    _en mname
message> ;

: announce-lightning-damage { _en _damage -- }
<message white memit
    m" lightning strikes "
    _en mname
    m"  for "
    _damage m.
    m"  damage."
message> ;

: announce-no-nearby-target ( -- )
<message red memit
    m" no nearby target"
message> ;

: announce-targeting ( -- )
<message white memit
    m" targeting..."
message> ;

: announce-targeting-cancelled ( -- )
<message white memit
    m" cancelled"
message> ;

: announce-must-target-in-fov ( -- )
<message yellow memit
    m" cancelled; cannot target there"
message> ;

: announce-no-target ( -- )
<message yellow memit
    m" cancelled; no target there"
message> ;

: announce-fireball ( -- )
<message orange memit
    m" the fireball explodes!"
message> ;

: announce-fire-damage { _en _damage -- }
<message orange memit
    m" fire scorches "
    _en mname
    m"  for "
    _damage m.
    m"  damage."
message> ;

: announce-no-inventory ( -- )
<message yellow memit
    m" no items"
message> ;

: announce-confused { _en -- }
<message light-green memit
    _en mname
    m"  looks confused"
message> ;

: announce-confusion-over { _en -- }
<message red memit
    _en mname
    m"  looks fine again"
message> ;

: announce-saved-game ( -- )
<message green memit
    m" game saved"
message> ;

: announce-loaded-game ( -- )
<message green memit
    m" game loaded"
message> ;

: announce-new-game ( -- )
<message white memit
    m" welcome to forthrogue!"
message> ;
