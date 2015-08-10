Accelerated Delivery
========================

*Accelerated Delivery* is a project I ([Alex Van Liew][at])) made with friends
[Jonathan Fischer][jt], [Scott Campbell][st], and Simeon King during the better
part of high school as part of the then-studio [Two Button Crew][tbc]. We 
self-published the game at <www.accelerateddeliverygame.com> in August of 2013, 
but fell flat, as so many indies do these days, so now we're opting to publish
the source code and release the game for free.  
The game itself is about unconvential mechanisms of package delivery.

[at]: www.twitter.com/_snoozbuster
[jt]: www.twitter.com/jon_fisch
[st]: www.twitter.com/studmuffinscott
[tbc]: www.twobuttoncrew.com

Building the Source
=======================

To build most of the projects, the [XNA Framework][xna] is required. Unfortunately,
the XNA Framework is deprecated and only supported on Visual Studio 2010. If you 
happen to have a copy of VS2010, pick up [XNA Game Studio 4.0 Refresh][xna-dev] and
install it (on Win8+ you may need to install the Games for Windows Live redistrutable
first). Then, follow [these steps][xna-install] to pull the extension forward to your
preferred version of VS (VS2012 and 2013 are mentioned, the version number for 2015 
is 14.0). 

You will also need the following libraries:

- [BEPUphysics (version 1.3.0)][bepu]
- [DPSF(version 2.5.0)][dpsf]

These aren't included with the source, because they aren't mine. Additionally, you'll
need to download the appropriate version of BEPUphysics and add a `Clear()` method to
the `Space` class, which removes all the entities in the `Space`. Build the x86 Release
version of the library.

Put BEPUphysics (and its XML file, if you plan to work on AD's source) at 
`Accelerated_Delivery_Win/BEPUphysics/x86/Release/` and DPSF at
`Accelerated_Delivery_Win/DPSF/`. If you want to locate the libraries elsewhere, you'll
need to update the solution file once you open it.

The game is currently missing the two credits videos because I tried to obfuscate them 
and ended up not backing them up because of it. Without them the game will crash during
loading. 

[xna]: https://msdn.microsoft.com/en-us/library/bb200104.aspx
[xna-dev]: https://msdn.microsoft.com/en-us/library/bb200104.aspx
[xna-install]: http://stackoverflow.com/a/10881007
[bepu]: http://bepuphysics.codeplex.com
[dpsf]: http://xnaparticles.com/Download.php

Building the Installers
=========================

The installers are built using [Nullsoft Scriptable Install System][nsis] and the scripts
to build them are located in `Accelerated_Delivery_Win/NSIS`. One script will build the
full installer, and the other will build the web installer; the web installer just downloads
the full installer from a given URL and runs it. The full installer's paths are hard-coded
and I need to go back and fix that up, so it won't run on your computer right now without
modifications.

The full installer will also make sure that the proper XNA Framework redistrutable is installed
and verify that the .NET Framework 4.5 is installed.

[nsis]: http://nsis.sourceforge.net/Main_Page

Projects
===========

The solution contains a number of projects which all have different uses and behaviors.

`Accelerated_Delivery.csproj`
---------------------------------

This project contains the DLL that is the core of the game. Theoretically, you could use solely
this DLL and create a game very similar to Accelerated Delivery by supplying your own levels,
assets, menus, and so on. In fact, I have used this DLL in my [Ludum][ld28] [Dare][ld29] 
[games][ld30], as the 2D portions of the library are fairly solid, and the rendering engine
is workable. This DLL inspired the (attempted) creation of my [Apathy Engine][ae] project.

This DLL was refactored from the core game to attempt to reduce code duplication between the
full game and the demo (which no longer exists, since the game is free), as well as to help
facilitate adding mods or other user customization and to help reduce technical debt if we
decided to make a sequel.

[ld28]: http://github.com/snoozbuster/ld-28
[ld29]: http://github.com/snoozbuster/ld-29
[ld30]: http://github.com/snoozbuster/ld-30
[ae]: http://github.com/snoozbuster/apathy-engine

`Accelerated_Delivery_Win.csproj`
-------------------------------------

This is the project that builds the game executable. It has an associated content project which
contains and builds all the assets. It won't load at all without the XNA Framework installed; if
you want to get it to load (although building obviously won't work), comment out line 5
(`<ProjectTypeGuids>` node) and line 329 (`<Import>` node with a XNA Framework target). You may find
that `<ProjectTypeGuids>` node may need to be deleted outright.

`Launcher.csproj`
------------

This builds the launcher for the game, which handles authentication and updating the game. It can
even update itself, using a collaboration between itself and the downloader helper.

`Downloader.csproj`
---------------------

This project builds the downloader application, which can update the game. It's used to avoid
requiring the launcher to request elevation just to download an update which may or may not exist.

`Decryptor.csproj`
---------------------

This project builds a tool that allows you to encrypt and decrypt save files (providing you know 
the username and password that was used to encrypt it). It's not required to build, but it can
help when debugging save problems.

`Accelerated_Console.csproj`
-------------------------------

This project builds a small text-only spinoff game that focuses on strategy and planning instead of
timing and multitasking. It's very simple, I made it on a whim one day and didn't go back, so it's
probably pretty easy.

Licenses
===========

The 3D models and 2D assets (but not the textures) are copyright Jonathan Fischer, and placed under 
a Noncommercial Attribution license. This more or less means that you can use them for anything you
like, but not claim them as yours or resell them. The code is not under any particular license, but
you are free to use it as long as you don't use large chunks of it or just copy the whole thing.
Be reasonable about it, and it would be nice if you placed a comment near the chunk you grabbed letting
anyone else know where you got it. The textures, music and sound effects are only licensed to use 
for building and playing Accelerated Delivery.

Copyright
=============

3D models are copyright Jonathan Fischer. 2D assets are copyright Alex Van Liew/Jonathan Fischer.
Level design copyright Simeon King and Scott Campbell. Concept copyright Jonathan Fischer. Code
copyright Alex Van Liew. Copyright 2011-2015.

DISCLAIMER
=============

A lot of this code is really, really bad. It was written as I learned C# and over a period of
multiple years without source control. Some of it is decent, some of it is really, really bad.
Take anything written here with a grain of salt.
