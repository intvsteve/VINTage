=============================================================================
INTV.jzIntv
=============================================================================

The INTV.jzIntv assembly defines features to access utilities provided by
Joe Zbiciak's SDK-1600. It is designed to provide such functionality to
various Intellivision (R)-related software projects that fall under the
general VINTage banner.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

This assembly was initially developed as part of a learning exercise for
new .NET 4.5 language features - specifically the async/await capabilities
it introduced. It was also part of a front-end for the jzIntv emulator. The
emulator-facing aspects have been largely removed, with the library's role
primarily turning to a means to access jzIntv's (and the SDK-1600's)
command-line tools as needed for the LTO Flash! user interface software.
This limited role may change in future updates to again encompass a more
complete role as a jzIntv front end.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.jzIntv project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.jzIntv.desktop.csproj: Used for Windows Vista and later
------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets.

INTV.jzIntv.xp.csproj: Used for Windows xp
------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.jzIntv.Mac.csproj: Used for Mac OS X
-----------------------------------------
This version of the project is used to build for Mac OS X deployments. It
has been developed Xamarin Studio 5.8.3. You can find Xamarin Studio at:
http://xamarin.com/download

Development of the Mac OS X version of the software was done on a MacBook
Pro running Mac OS X 10.8.5. Versions of Xamarin Studio newer than 5.8.3
dropped support for Mac OS X 10.8. It is unknown if there are compatibility
issues with this project in newer versions of Xamarin Studio.

NOTE:
-----
Because the Mac OS X projects make use of p/Invoke to call native
system libraries, at this time the Mac version of the software must use
the MonoMac libraries, rather than Xamarin.Mac, to remain free. The standard
free version of Xamarin.Mac does not support p/Invoke. Also, as a
consequence of this, the Mac software runs as a 32-bit application. This may
change in the future.

NOTE:
-----
With Microsoft's acquisition of Xamarin in March of 2016, the opportunity
to port the Mac version to full Xamarin.Mac now exists!

NOTE:
-----
Binaries for the following tools from the SDK-1600 ARE NOT included in the
source distribution, nor committed to the source repository:
  bin2luigi
  bin2rom
  intvname
  luigi2bin
  rom_merge
  rom2bin
  rom2luigi

The above tools are REQUIRED for this library to function! These tools can
be provided via several mechanisms as detailed here.

I: Manual Installation
----------------------
Place the above executables into the appropriate subdirectory relative to
the directory containing this file:
  Linux:   tools/Linux
  Mac:     tools/Mac
  Windows: tools

You can find these tools in SDK-1600 distributions located here:
  http://spatula-city.org/~im14u2c/intv/

II: Install via make During Build
---------------------------------
These binaries may be retrieved by the project build if not available locally,
if you configure your system appropriately. Consult the 'custom_jzintv.mak'
file in the root directory for documentation regarding how to configure the
build to retrieve the necessary tools from SDK-1600.

On Mac and Linux platforms, it is presumed that standard tools are already
available or can be installed via the platform's package manager. These
tools are:
  GNU make
  curl
  unzip

A truly clean Windows build will REQUIRE a valid MSYS2 environment to be
configured on your Windows system and that it have the aforementioned tools
installed in it.

The following notes attempt to provide guidance on setting up from a clean
MSYS2 environment on a Windows system so that  you can invoke the make files
the build uses:

  pacman -S base-devel
  pacman -S curl
  pacman -S unzip

This minimal setup is enough to have the build download a jzIntv binary
distribution and extract the necessary files so this library can function.
The default download location used to retrieve a SDK-1600 distributions is
declared in the custom_jzintv.mak file, and is:
  http://spatula-city.org/~im14u2c/intv/

You may alter the settings in that makefile as appropriate.

III: Build from Source and Install via make During Build
--------------------------------------------------------
If you wish to build the tools from source, you may do so. Two options exist:
  1. Download a source distribution, such as the one available here:
       http://spatula-city.org/~im14u2c/intv/
  2. Specify some other source repo, e.g. the official SVN repo if one has
     access, or some other form of source control or simple local copy

To build the SDK-1600 source in MSYS2 in a Windows environment, you will
also need the following packages in addition to those listed above:

  pacman -S mingw-w64-i686-gcc
  pacman -S mingw-w64-i686-readline

Note that at this time, only 32-bit builds of jzIntv for Windows are
supported. That said, if you're setting up your environment just prepare
for the future and install the 64-bit packages as well:

  pacman -S mingw-w64-x86_64-gcc
  pacman -S mingw-w64-x86_64-readline

It will probably also be necessary to have one or both of the SVN and Git
source control tools installed on your system as well.

To build from the SDK-1600 source, you must provide provide configuration
data so this project can build the tools. Ensure that you are able to build
in a standard command line environment (or MSYS2 on Windows). Once you have
ensured you can build, it is relatively simple to integrate building the tools
locally by defining variables in these files:
  custom.mak (all platforms)
  custom_jzIntv.mak (all platforms)
  custom.bat (Windows)

These files can be found in the root directory. Instructions for the necessary
changes are included in custom.mak, custom_jzIntv.mak and custom.bat files
respectively.

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

NOTE:
-----
Ensure that the programs in the tools/Mac and / or tools/Linux subdirectory have
properly retained the executable bit. If these programs do not have execute
permissions, Mono may crash in native code when attempting to execute the program
from C#. See the bug report here:
See the bug report here:

  https://bugzilla.xamarin.com/show_bug.cgi?id=37138

The INTV.Shared component guards against this problem when launching external
programs and reports the problem by throwing System.InvalidOperationException.

DEPENDENCIES
=============================================================================
The INTV.jzIntv project depends on one other assembly that is part of the
general VINTage umbrella. In addition to standard system libraries,
INTV.jzIntv requires the following:

INTV.Core

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - Model: Provides a software model to drive the jzIntv emulator and access
          various SDK-1600 command line utilities that it includes. Although
          this provides a way to launch jzIntv, it does not provide a UI from
          which to do so. See the INTV.jzIntvUI project for that capability.

 - Properties: Contains assembly information.

 - Resources: Contains strings intended for display in the user interface.

 - tools: This directory contains a snapshot of various tools from the SDK-1600
          that are supported by the library. The larger VINTage software suite
          makes use of these tools.
          from another location so support for updated versions of jzIntv
          can be easily accomplished.

NOTE: INTV.jzIntv provides no user-interface-facing elements.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
