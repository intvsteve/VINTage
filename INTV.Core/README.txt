=============================================================================
INTV.Core
=============================================================================

The INTV.Core assembly defines basic types, interfaces, and utilities for
use with various Intellivision (R)-related software projects that fall under
the general VINTage banner.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

This assembly started as the core of a C# experiment to create another
Intellivision emulator back in 2012. Because two of the target platforms
under consideration were Windows Phone and Windows RT, you will find that
the INTV.Core.pcl assembly is limited to small subset of the .NET platform
of that era. Additionally, because the library's role has grown to include
support for Windows xp, language support is limited to .NET 4.0 features.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.Core project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.Core.pcl.csproj: Used for Windows Vista and later
------------------------------------------------------
The Portable Class Library project was created using Microsoft Visual
Studio Ultimate 2012. This project cannot be built using Microsoft Visual
Studio Express 2012. Perhaps this will change in newer product releases.
This version of the project is used for the Desktop builds, which target
Microsoft Windows Vista and later.

INTV.Core.xp.csproj: Used for Windows xp
----------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.Core.Mac.csproj: Used for Mac OS X
---------------------------------------
This version of the project is used to build for Mac OS X deployments. It
has been developed using MonoMac 4.2.1.102 and Xamarin Studio 5.8.3. You
can find MonoMac at: http://www.mono-project.com/download/ and Xamarin
Studio at: http://xamarin.com/download

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
The INTV.Core project does not depend on any libraries aside from standard
system libraries available with the build environment. For the Mac OS X build,
it requires MonoMac to access core Cocoa types, such as NSObject.

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - ComponentModel: Abstractions of the basic 'property changed' interface
                   commonly used in C# / WPF programming, which is also
                   analogous to the KVO (Key-Value-Observer) model used
                   in Cocoa interfaces on Apple's platforms. Also declares
                   the type RelayCommandBase, used as the basic
                   implementation of the widely used RelayCommand pattern.

 - Model: Interfaces, types, and implementations for many of the core
          elements relating to Intellivision-related software. Chief among
          these is the IRom interface, an abstraction for dealing with the
          commonly active, but functionally different, ROM file formats used
          by emulators and cartridge emulation hardware such as the
          Intellicart, Cuttle Cart 3, LTO Flash!. These ROM formats include:
          .bin + .cfg, .int, .itv, .rom, .cc3, and .luigi

          The Model.Program namespace provides a rich set of types used to
          describe the details of program ROMs, including special hardware
          requirements and other trivia. This namespace also includes a
          simple database mechanism for managing a ROM library.

          The Model.Resources namespace contains a snapshot of a transformed
          version of the game information database used to drive the
          INTV Funhouse website. This database can act as a de-facto source
          of ROM information.

 - Properties: Contains assembly information.

 - Resources: Contains strings intended for display in the user interface.

 - tools: The tools directory contains stock configuration files for working
          with game ROMs. The .bin ROM format consists of the raw ROM data,
          with a companion file, the .cfg file, which describes how to map
          the ROM into Intellivision memory. Over time, the .cfg format has
          expanded to include other descriptive information, such as
          additional hardware requirements and even ROM patch code.

 - Utility: This namespace contains general-purpose functions and types,
            such as various checksum functions that are commonly used for
            data integrity checks and ROM identification. Other utilities
            provide callback function registration facilities to assist
            with operating with files. (.NET Portable Class Libraries do
            not have direct access to the file system, but do operate on data
            streams, for example.)

NOTE: INTV.Core provides no user-interface-facing elements.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
