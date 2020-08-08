\ A weightttable is a structure in memory like this:
\   max | chance | xt | chance | xt ...

: get-table-entry { _table _roll -- xt }
    _table cell+ begin
        dup @ _roll u> if
            cell+ @ exit
        then cell+ cell+
    again
;

: pick-random-table ( randtable -- xt )
    0 over @ randint get-table-entry
;

: randtable: ( "name" -- addr )
    create 0 , latestxt execute
    does> pick-random-table
;

: table: ( "name" -- addr )
    create 0 , latestxt execute
    does> swap get-table-entry
;

: endtable ( addr -- )
    2 swap -!
;

: entry: ( addr chance "xt" -- addr )
    over @ + dup , bl parse find-name ?dup-if
        name>int ,
        over !
    else abort" invalid name" then
;

: rawentry ( addr xt chance -- addr )
    swap , ,
;
