# Developer Changelog
For tracking changes between commits. Intended only for the developer.
In other words, if you are not Logiq, ignore this.

### Changes:
- Added more syntax highlighting to the Debug class
  - Separated syntax highlighting for bools (true vs false)
  - Added syntax highlighting for types
    - The game can now register custom types to include in this highlighting rule
  - Improved the numerical syntax highlighting regex
    - Now works with numbers prefixed by + or - or a period for decimal numbers
    - Allows for any number of (optional) commas, periods and semicolons within the number
    - Won't highlight a period that comes after a number unless a number comes after it
- Implemented the identifier as a custom type (a wrapper for a string)
  - Validation will happen once upon creation, removing the need to check validation each time it is used
  - Is implicitly convertible between `identifier` and `string` 
- Implemented a simple registry system
  - Registries can be created as a field or a class
  - Class registries (such as `TextureRegistry`) can use a static instance to allow easier access to its functions, so as to not require going through an instance to call into the functions

## Tasks to complete:

- [ ] Add an extensible version system
- [ ] Create an unmanaged resource tracker class
- [x] Create a Texture class (Incomplete, doesn't actually load or create a texture! Just a test for the registry and identifier systems)
- [ ] Create a Shader class

## Completed:
New:
- [x] Create an identifier type
- [x] Create a Registry class
- [x] Create a TextureRegistry class

Old:
- [x] Add a rudimentary version system
- [x] Add a "%version%" flag to the game window title formatting string
- [x] Implement target FPS changing
- [x] Make sure before rendering the screen is cleared, and after rendering the window swaps the buffers. Also do this for when rendering while resizing
- [x] Make rendering while resizing toggleable via program arguments (flags)
- [x] Implement indentation in the Debug class