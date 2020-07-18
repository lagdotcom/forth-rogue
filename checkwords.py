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
    'core': set(['!', '#', '#>', '#s', "'", '(', '*', '*/', '*/mod', '+', '+!', '+loop', ',', '-', '.', '."', '/', '/mod', '0<', '0=', '1+', '1-', '2!', '2"', '2/', '2@', '2drop', '2dup', '2over', '2swap', ':', ';', '<', '<#', '=', '>', '>body', '>in', '>number', '>r', '?dup', '@',  '[', "[']", '[char]', ']', 'abort', 'abort"', 'abs', 'accept', 'align', 'aligned', 'allot', 'and', 'base', 'begin', 'bl', 'c!', 'c,', 'c@', 'cell+', 'cells', 'char', 'char+', 'chars',  'constant', 'count', 'cr', 'create', 'decimal', 'depth', 'do', 'does>', 'drop', 'dup', 'else', 'emit', 'environment?', 'evaluate', 'execute', 'exit', 'fill', 'find', 'fm/mod', 'here', 'hold', 'i', 'if', 'immediate', 'invert', 'j', 'key', 'leave', 'literal', 'loop', 'lshift', 'm*', 'max', 'min', 'mod', 'move', 'negate', 'or', 'over', 'postpone', 'quit', 'r>', 'r@', 'recurse', 'repeat', 'rot', 'rshift', 's"', 's>d', 'sign', 'sm/rem', 'source', 'space', 'spaces', 'state', 'swap', 'then', 'type', 'u.', 'u<', 'um*', 'um/mod', 'unloop', 'until', 'variable', 'while', 'word', 'xor']),
    'core-ext': set(['.(', '.r', '0<>', '0>', '2>r', '2r@', ':noname', '<>', '?do', 'action-of', 'again', 'buffer:', 'c"', 'case', 'compile,', 'defer', 'defer!', 'defer@', 'endcase', 'endof', 'erase', 'false', 'hex', 'holds', 'is', 'marker', 'nip', 'of', 'pad', 'parse', 'parse-name', 'pick', 'refill', 'restore-input', 'roll', 's\\"', 'save-input', 'source-id', 'to', 'true', 'tuck', 'u.r', 'u>', 'unused', 'value', 'within', '[compile]', '\\']),
    'block': set(['blk', 'block', 'buffer', 'flush', 'load', 'save-buffers', 'update']),
    'block-ext': set(['empty-buffers', 'list', 'scr', 'thru']),
    'double': set(['2constant', '2literal', '2variable', 'd+', 'd-', 'd.', 'd.r', 'd0<', 'd0=', 'd2*', 'd2/', 'd<', 'd=', 'd>s', 'dabs', 'dmax', 'dmin', 'dnegate', 'm*/', 'm+']),
    'double-ext': set(['2rot', '2value', 'du<']),
    'exception': set(['catch', 'throw']),
    'facility': set(['at-xy', 'key?', 'page']),
    'facility-ext': set(['+field', 'begin-structure', 'cfield:', 'ekey', 'ekey>char', 'ekey>fkey', 'ekey?', 'emit?', 'end-structure', 'field:', 'k-alt-mask', 'k-ctrl-mask', 'k-delete', 'k-down', 'k-end', 'k-f1', 'k-f2', 'k-f3', 'k-f4', 'k-f5', 'k-f6', 'k-f7', 'k-f8', 'k-f9', 'k-f10', 'k-f11', 'k-f12', 'k-home', 'k-insert', 'k-left', 'k-next', 'k-prior', 'k-right', 'k-shift-mask', 'k-up', 'ms', 'time&date']),
    'file': set(['bin', 'close-file', 'create-file', 'delete-file', 'file-position', 'file-size', 'include-file', 'included', 'open-file', 'r/o', 'r/w', 'read-file', 'read-line', 'reposition-file', 'resize-file', 'w/o', 'write-file', 'write-line']),
    'file-ext': set(['file-status', 'flush-file', 'include', 'rename-file', 'require', 'required']),
    'float': set(['>float', 'd>f', 'f!', 'f*', 'f+', 'f-', 'f/', 'f0<', 'f0=', 'f<', 'f>d', 'f@', 'falign', 'faligned', 'fconstant', 'fdepth', 'fdup', 'fliteral', 'float+', 'floats', 'floor', 'fmax', 'fmin', 'fnegate', 'fover', 'frot', 'fround', 'fswap', 'fvariable', 'represent']),
    'float-ext': set(['df!', 'df@', 'dfalign', 'dfaligned', 'dffield:', 'dfloat+', 'dfloats', 'f**', 'f.', 'f>s', 'fabs', 'facos', 'facosh', 'falog', 'fasin', 'fasinh', 'fatan', 'fatan2', 'fatanh', 'fcos', 'fcosh', 'fe.', 'fexp', 'fexpm1', 'ffield:', 'fln', 'flnp1', 'flog', 'fs.', 'fsin', 'fsincos', 'fsinh', 'fsqrt', 'ftan', 'ftanh', 'ftrunc', 'fvalue', 'f~', 'precision', 's>f', 'set-precision', 'sf!', 'sf@', 'sfalign', 'sfaligned', 'sffield:', 'sfloat+', 'sfloats']),
    'locals': set(['(local)', 'locals|', '{:']),
    'memory': set(['allocate', 'free', 'resize']),
    'tools': set(['.s', '?', 'dump', 'see', 'words']),
    'tools-ext': set([';code', 'ahead', 'assembler', 'bye', 'code', 'cs-pick', 'cs-roll', 'editor', 'forget', 'n>r', 'name>compile', 'name>interpret', 'name>string', 'nr>', 'synonym', 'traverse-wordlist', '[defined]', '[else]', '[if]', '[then]', '[undefined]']),
    'search': set(['definitions', 'forth-wordlist', 'get-current', 'get-order', 'search-wordlist', 'set-current', 'set-order', 'wordlist']),
    'search-ext': set(['also', 'forth', 'only', 'order', 'previous']),
    'string': set(['-trailing', '/string', 'blank', 'cmove', 'cmove>', 'compare', 'search', 'sliteral']),
    'string-ext': set(['replaces', 'substitute', 'unescape']),
    'xchar': set(['x-size', 'xc!+', 'xc!+?', 'xc', 'xc-size', 'xc@+', 'xchar+', 'xemit', 'xkey', 'xkey?']),
    'xchar-ext': set(['+x/string', '-trailing-garbage', 'ekey>xchar', 'x-width', 'xc-width', 'xchar-', 'xhold', 'x\string-']),
    'gforth': set(['%alloc', '%size', '-rot', '<=', '>=', '?dup-if', '[endif]', '[ifdef]', '[ifundef]', '[?do]', '[do]', '[for]', '[loop]', '[+loop]', '[next]', '[begin]', '[until]', '[again]', '[while]', '[repeat]', 'cell', 'cell%', 'char%', 'emit-file', 'end-struct', 'f=', 'field', 'form', 'off', 'on', 'struct', 'under+', 'utime', '{'])
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
