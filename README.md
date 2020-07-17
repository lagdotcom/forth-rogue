# forth-rogue

It's a roguelike! In Forth!

## Compatibility

Given that this is a Forth program, it should be portable. However, currently it relies on a few gforth-specific extensions (like `struct`) so that's the preferred Forth. Also however, gforth for Windows uses the cygwin translation layer, which breaks ANSI compatibility with modern Windows terminals. If you're on Windows, I recommend running forth-rogue through the Windows Subsystem for Linux.

## Getting started

You'll need [gforth](https://www.gnu.org/software/gforth/). I have no idea what version is required, but I'm running 0.7.3. Then, just run it with `./rogue`.
