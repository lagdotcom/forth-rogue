include random.fs

: randint ( min max -- n )
    1+ over - random +
;
