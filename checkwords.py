import glob


def readtosp(f):
    s = ''
    ch = f.read(1)
    while ch not in ' \t\r\n':
        s += ch
        ch = f.read(1)
    return s


def readtonl(f):
    s = ''
    ch = f.read(1)
    while ch not in '\r\n':
        s += ch
        ch = f.read(1)
    return s


def isnum(w):
    if w.isdigit():
        return True
    if w[0] == '-' and w[1:].isdigit():
        return True
    if w[-1] == 'e' and w[:-1].isdigit():
        return True
    return False


def maybeaddw(l, w):
    if w[0] != '$' and not isnum(w):
        l.add(w)


quotemodes = {'{': '}', '(': ')', '."': '"', 's"': '"',
              'c"': '"', 'm"': '"', 'message"': '"'}
wordlists = {
    'core': set(['!', '#>', '#s', "'", '(', '*', '+', '+!', '-', '.', '."', '/', '/mod', '0=', '1+', '1-', '2drop', '2dup', ':', ';', '<', '<#', '=', '>', '>r', '@', '[', "[']", '[char]', ']', 'abs', 'align', 'allot', 'and', 'begin', 'bl', 'c!', 'c,', 'c@', 'cell+', 'cells', 'char', 'char+', 'chars', 'cmove', 'constant', 'count', 'cr', 'create', 'decimal', 'do', 'drop', 'dup', 'else', 'emit', 'execute', 'exit', 'hex', 'hold', 'i', 'if', 'immediate', 'j', 'literal', 'loop', 'max', 'min', 'negate', 'nip', 'or', 'over', 'postpone', 'r>', 'r@', 'repeat', 'rot', 's"', 's>d', 'swap', 'then', 'type', 'unloop', 'until', 'variable', 'while']),
    'core-ext': set(['0>', ':noname', '<>', '?do', 'c"', 'case', 'endcase', 'endof',
                     'false', 'fill', 'of', 'parse', 'to', 'true', 'tuck', 'value', 'within']),
    'exception': set(['throw']),
    'string': set(['sliteral']),
    'tools': set(['.s']),
    'double': set(['d=', 'dabs']),
    'float': set(['d>f']),
    'float-ext': set(['fsqrt']),
    'tools-ext': set(['bye']),
    'memory': set(['allocate', 'free']),
    'file': set(['close-file', 'create-file', 'w/o', 'write-file', 'write-line']),
    'facility': set(['at-xy']),
    'facility-ext': set(['ekey', 'ekey>char']),
    'ekeys': set(['ekey>fkey', 'k-down', 'k-left', 'k-right', 'k-up', 'k-shift-mask']),
    'gforth': set(['%alloc', '%size', '-rot', '<=', '>=', '?dup-if', '[endif]', '[ifdef]', '[ifundef]', 'cell', 'cell%', 'char%', 'defer', 'emit-file', 'end-struct', 'f=', 'field', 'form', 'include', 'is', 'off', 'on', 'struct', 'under+', 'utime', '{'])
}


def scanwords(fn):
    w = set()
    f = open(fn, 'r')

    cw = ''
    ch = f.read(1)
    qm = None
    while ch:
        if ch == qm:
            qm = None
        elif qm:
            pass
        elif ch == '\\':
            readtonl(f)
        elif ch in ' \t\r\n':
            if cw:
                if cw == 'include':
                    readtonl(f)
                elif cw == '[char]' or cw == 'char':
                    readtosp(f)

                maybeaddw(w, cw)
                if cw in quotemodes:
                    qm = quotemodes[cw]
            cw = ''
        else:
            cw += ch.lower()
        ch = f.read(1)

    if cw:
        maybeaddw(w, cw)

    return w


if __name__ == '__main__':
    words = set()
    for fn in glob.glob('*.fs'):
        words = words.union(scanwords(fn))
    wls = {}
    for w in sorted(words):
        wwl = 'unknown'
        if w[0] == '_':
            wwl = '(local)'
        for (name, wl) in wordlists.items():
            if w in wl:
                wwl = name
        if wwl not in wls:
            wls[wwl] = []
        wls[wwl].append(w)
    for name in sorted(wls.keys()):
        print(name+':', ' '.join(wls[name]) + '\n')
