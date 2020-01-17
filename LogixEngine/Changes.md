﻿# Developer Changelog
For tracking changes between commits. Intended only for the developer.
In other words, if you are not Logiq, ignore this.

### Initial tasks completed:

- Written the basic startup sequence for the engine.
  - Formatted console printing system (Debug class)
    - Prints debug messages when the game is run with the '-debug' flag
    - Debug messages have syntax highlighting for some types:
      - Numbers
      - Identifiers
      - Strings
    - When the game is not running in -debug mode, only prints exceptions
      in the console and does not use formatting/syntax highlighting
    - In -debug mode, everything is printed  in columns, to make it easy 
      to read
    - In -debug mode, each line has a line number and timestamp
  - Basic Game class API; makes creating a game as easy as extending
    the class and implementing the functions, and then calling a single
    line of code from the entry-point!
  - Game window title updating; the game window's title can display 
    information using a string format containing any of the following
    flags:
    - %name%: the game's display name
    - %cps%: the current cycles (updates) per second
    - %tcps%: the target cycles (updates) per second
    - %fps%: the current frames per second
    - %tfps%: the target frames per second
  - Console title reversion; when the game ends the console returns to 
    how it was in case the user plans to continue using it
  - Shutdown system which should allow for careful cleanup of unmanaged
    resources such as OpenGL objects

### Tasklist:

- [ ] Add an extensible version system
- [ ] Add an extensible version system
- [ ] Add a "%version%" flag to the game window title formatting string
- [ ] Implement target FPS changing
- [ ] Make sure before rendering the screen is cleared, and after rendering the window swaps the buffers. Also do this for when rendering while resizing
- [ ] Make rendering while resizing toggleable via program arguments (flags)