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

 - Model: Provides a simple software model to drive the jzIntv emulator and
          access various command line features supported by the emulator.

 - Properties: Contains assembly information.

 - Resources: Contains strings intended for display in the user interface.

NOTE:
-----
This project supports locally building the jzIntv emulator to include in
the installed version of the overall project.

If you also have the sources for jzIntv, you can OPTIONALLY provide local
configuration values to have this project locally build the emulator,
rather than using a copy downloaded or built elsewhere. To do this, you must
already be able to build jzIntv. Once you have ensured you can locally build
jzIntv, it is a simple matter to integrate building it locally by modifying
the files named:
  custom.mak (all platforms)
  build_jzIntv.bat (Windows)
which can be found in the same directory as this REAMDE file. Instructions
for the necessary changes are included in custom.mak and build_jzIntv.bat
respectively.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.

