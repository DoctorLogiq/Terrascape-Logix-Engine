# Developer Changelog
For tracking changes between commits. Intended only for the developer.
In other words, if you are not Logiq, ignore this.

### Changes
- Implemented Debug indentation
- Implemented Debug profiling and written strong documentation for it
- Added documentation for all functions in the Debug class
- Added a version string to the game class
  - This can now be shown in the titlebar using `%version%`
  - This will be made into a proper version system in the future

### Tasklist:

- [x] Add a rudimentary version system
- [ ] Add an extensible version system
- [x] Add a "%version%" flag to the game window title formatting string
- [x] Implement target FPS changing
- [x] Make sure before rendering the screen is cleared, and after rendering the window swaps the buffers. Also do this for when rendering while resizing
- [x] Make rendering while resizing toggleable via program arguments (flags)
- [x] Implement indentation in the Debug class