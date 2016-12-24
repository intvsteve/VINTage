=============================================================================
INTV.jzIntv
=============================================================================

The INTV.jzIntv assembly defines features to access utilities provided by
Joe Zbiciak's jzIntv emulator. It is designed to provide such functionality
to various Intellivision (R)-related software projects that fall under the
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
In order to provide a fully functional out-of-the-box build, binaries for
the following tools are included in the source distribution:
  bin2luigi
  bin2rom
  intvname
  luigi2bin
  rom2bin
  rom2luigi

If you also have the sources for jzIntv, you can OPTIONALLY provide local
configuration values to have this project locally build the utility programs,
rather than using those included with this component. To do this, you must
already be able to build jzIntv -- or at least the programs listed above.
Once you have ensured you can locally build these, it is a simple matter
to integrate building these tools locally by modifying the files named:
  custom.mak (all platforms)
  build_tools.bat (Windows)
which can be found in the same directory as this REAMDE file. Instructions
for the necessary changes are included in custom.mak and build_tools.bat
respectively.

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

NOTE:
-----
If you've retrieved the sources from a source control repository, you must
ensure that the programs in the tools/Mac subdirectory have properly retained
the executable bit. If these programs do not have execute permissions, Mono
may crash in native code when attempting to execute the program from C#.
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
          various command line utilities that it includes. The jzIntv driver
          is not presently in use. (See comments above.)

 - Properties: Contains assembly information.

 - Resources: Contains strings intended for display in the user interface.

 - tools: This directory contains a snapshot of various tools that ship with
          jzIntv that are currently supported by the library. Future updates
          will provide a means to configure the library to use the tools
          from another location so support for updated versions of jzIntv
          can be easily accomplished.

NOTE: INTV.jzIntv provides no user-interface-facing elements at this time.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Productions.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Productions.
