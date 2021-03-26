=============================================================================
INTV.jzIntvUI
=============================================================================

The INTV.jzIntvUI assembly defines features to run Intellivision (R) ROMs in
Joe Zbiciak's jzIntv emulator.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision (R) emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

The code in this assembly was initially part of a learning exercise for
new .NET 4.5 language features - specifically the async/await capabilities
it introduced.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.jzIntv project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.jzIntvUI.desktop.csproj: Used for Windows Vista and later
--------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets.

INTV.jzIntvUI.xp.csproj: Used for Windows xp
--------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.jzIntvUI.Mac.csproj: Used for Mac OS X
-------------------------------------------
This version of the project is used to build for 32-bit Mac OS X
deployments. It has been developed using MonoMac 4.2.1.102 and Xamarin
Studio 5.8.3. Good luck finding those old tools!

Development of the 32-bit Mac OS X version of the software was done on a
MacBook Pro running Mac OS X 10.8.5. Versions of Xamarin Studio newer than
5.8.3 dropped support for Mac OS X 10.8. 

INTV.jzIntvUI.XamMac.csproj: Used for macOS
-------------------------------------------
This project was the initial port to Xamarin.Mac after it became freely
available. It represents the transition from MonoMac to the early unified
Xamarin.Mac platform, prior to Xamarin Studio being rebranded to Visual
Studio for Mac after Microsoft's acquisition of Xamarin. For all practical
purposes, this is identical to the Visual Studio for Mac project.

INTV.jzIntvUI.VSMac.csproj: Used for macOS
------------------------------------------
Newer versions targeting 64-bit macOS are built using Visual Studio for Mac
and will only run in macOS 10.9 or later, and use this project.

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

DEPENDENCIES
=============================================================================
The INTV.jzIntvUI project depends on other assemblies that are part of the
general VINTage umbrella. In addition to standard system libraries,
INTV.jzIntvUI requires the following:

INTV.Core
INTV.jzIntv
INTV.Shared

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - Commands: jzIntv-specific commands to configure or execute the emulator.

 - Converter: Data converters used to present data types in the UI.

 - Model: Provides a simple software model to drive the jzIntv emulator and
          access various command line features supported by the emulator.

 - Properties: Contains assembly information.

 - Resources: Contains strings intended for display in the user interface.

 - View: This namespace contains implementations to provide a user interface
         to configure emulator features, specify core ROM locations needed
         to operate the emulator, et. al.

 - ViewModel: This namespace contains the ViewModels used to communicate
              between the model representing the emulator configuration
              and other model-to-view operations.

NOTE:
-----
This project supports inclusion of the jzIntv emulator via several
mechanisms, including via automatic download of binary or source
distributions as well as direct access to the jzIntv SVN repo. It does NOT
include a copy of the emulator and its support files directly in the source
repository!

Enabling one of the mechanisms available to automatically include jzIntv
provides for the redistribution of jzIntv in the installed version of the
overall project.

Several mechanisms to include jzIntv along with this project are outlined.

I: Manual Installation
----------------------
Place a copy of the necessary parts of a jzIntv distribution, preserving its
file system structure, into the appropriate subdirectory relative to the
directory containing this file:
  Linux:   jzIntv/Linux
  Mac:     jzIntv/Mac
  Windows: jzIntv/Win

You can find recent jzIntv distributions here:
  http://spatula-city.org/~im14u2c/intv/

II: Install via make During Build
---------------------------------
The necessary jzIntv binaries and companion files may be retrieved by the
project build if not available locally, if you configure your system
appropriately. Consult the 'custom_jzintv.mak' file in the root directory
for documentation regarding how to configure the build to retrieve the
necessary distribution.

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
distribution and extract the necessary files so this library can include the
jzIntv distribution. The default download location used to retrieve a jzIntv
distributions is declared in the custom_jzintv.mak file, and is:
  http://spatula-city.org/~im14u2c/intv/

You may alter the settings in that makefile as appropriate.

III: Build from Source and Install via make During Build
--------------------------------------------------------
If you wish to build jzIntv from source, you may do so. Two options exist:
  1. Download a source distribution from:
       http://spatula-city.org/~im14u2c/intv/
  2. Specify some other source repo, e.g. the official SVN repo if one has
     access, or some other form of source control or simple local copy

On all platforms, you need to ensure that the SDL2 and, optionally, SDL
development packages are available.

To build the jzIntv source in MSYS2 in a Windows environment, you will
also need the following packages:

  pacman -S mingw-w64-i686-SDL
  pacman -S mingw-w64-i686-SDL2
  pacman -S mingw-w64-i686-gcc
  pacman -S mingw-w64-i686-readline

Note that at this time, only 32-bit builds of jzIntv for Windows are
supported. That said, if you're setting up your environment just prepare
for the future and install the 64-bit packages as well:

  pacman -S mingw-w64-x86_64-SDL
  pacman -S mingw-w64-x86_64-SDL2
  pacman -S mingw-w64-x86_64-gcc
  pacman -S mingw-w64-x86_64-readline

Note that the above list specifies both SDL and SDL2. For all intents and
purposes, consider the SDL builds obsolete. It is mentioned only for
completeness.

It will probably also be necessary to have one or both of the SVN and Git
source control tools installed on your system as well.

To build from the jzIntv source, you must provide provide configuration
data so this project can build the emulator. Ensure that you are able to build
in a standard command line environment (or MSYS2 on Windows). Once you have
ensured you can build, it is relatively simple to integrate building jzIntv
locally by defining variables in these files:
  custom.mak (all platforms)
  custom_jzIntv.mak (all platforms)
  custom.bat (Windows)

These files can be found in the root directory. Instructions for the necessary
changes are included in custom.mak, custom_jzIntv.mak and custom.bat files
respectively.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
