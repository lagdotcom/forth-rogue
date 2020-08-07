: stairs! { _stairs _floor -- }
    _floor _stairs stairs-floor !
;

: add-stairs { _en _floor -- }
    _en ['] entity-stairs stairs% add-component
    _floor stairs!
;
