# Terrascape-Logix-Engine

A project for practising game development - making the game and its engine.

---

## What is this?

This is a personal project in which I am using as a platform to experiment with and learn OpenGL, and game development as a whole. *This is NOT a product*! The game itself is something that I've wanted to make for a really long time, so it's a passion project, but it's very unlikely it will be released. That said, that does *NOT* mean you can just go and steal the code or assets. The only reason this is even public at all is so that anyone who stumbles across it (who probably does so because they're trying to learn similar things themselves) can learn from it.

## I am no professional

All of the code here and the things I'm trying are not necessarily advisable. If you choose to learn from this project, double-check and second-guess *everything*, because chances are, I'm doing some fancy-pantsy stuff that should never be done in an actual public product.

## What's being used?

I am using a couple of libraries available via <a href="https://www.nuget.org">NuGet</a>, such as:

-   Colorful.Console [<a href="http://colorfulconsole.com">Website</a>] [<a href="https://www.nuget.org/packages/Colorful.Console/">NuGet</a>]
    -   Allows for printing in colour in the console, and using RegEx's to highlight certain things in colour
-   OpenTK [<a href="https://opentk.net/index.html">Website</a>] [<a href="https://www.nuget.org/packages/OpenTK/">NuGet</a>]
    -   The C# bindings for OpenGL, also provides a basic GameWindow class which runs the game loop for us
-   ImageSharp [<a href="https://sixlabors.com/projects/imagesharp/">Website</a>] [<a href="https://www.nuget.org/packages/SixLabors.ImageSharp/1.0.0-beta0007">NuGet</a>]
    -   Not absolutely necessary but this is used in the OpenTK documentation for texture loading, and from my experience is a nice and easy library to use  
-   FbxSharp [<a href="https://github.com/izrik/FbxSharp">GitHub</a>] [<a href="https://www.nuget.org/packages/FbxSharp/">NuGet</a>]
    -   Likely going to be used to load .fbx models, as .obj is very limited and I prefer .fbx for its ability to retain object structures

## Game flags
The game uses various flags to control how it starts up. These are subject to change, however, for development the flags include:
- `-console`: if the game is launched from the .exe, show a console where information will be printed to
- `-debug`: allows printing of extra information in the console; can be used instead of `-console`. Also enables some extra code such as assertions and performance profiling systems (these may ironically impact performance a tiny bit).
- `drwr`: short for "don't render while resizing"; does exactly what it says on the tin; prevents the game from rendering while you're resizing the window.

## Things I've learnt along the way

I appear to have come up with the 'perfect' regular expression for highlighting numbers:

```csharp
 @"[-+.]?\d+([\.:,]\d+)*"
```

This seems to work great in conjunction with Colorful.Console.
There is a demo & explanation available here:
<a href="https://regexr.com/4q810">RegExr</a>.

This RegEx will highlight numbers in all sorts of situations;
- Regular integers and decimal values
- Numbers prefixed by a plus, minus or period (for decimal numbers where the zero is not shown due to redundancy)  
- Numbers containing periods, commas or colons as separators

It will also not highlight numbers with a period after them, if there are no numbers immediately following it.
The RegEx has gone through many transformations and optimisations, and is now in a good place. 