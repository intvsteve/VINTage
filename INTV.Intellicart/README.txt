=============================================================================
INTV.Intellicart
=============================================================================

The INTV.Intellicart assembly defines features to work with the Intellicart
cartridge emulator from Chad Schell. It is designed to add such functionality
to various Intellivision (R)-related software projects that fall under the
general VINTage banner.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

This assembly was initially developed as an extension for use with the
LTO Flash! User Interface Software.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.Intellicart project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.Intellicart.desktop.csproj: Used for Windows Vista and later
-----------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets.

INTV.Intellicart.xp.csproj: Used for Windows xp
-----------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.Intellicart.Mac.csproj: Used for Mac OS X
----------------------------------------------
This version of the project is used to build for 32-bit Mac OS X
deployments. It has been developed using MonoMac 4.2.1.102 and Xamarin
Studio 5.8.3. Good luck finding those old tools!

Development of the 32-bit Mac OS X version of the software was done on a
MacBook Pro running Mac OS X 10.8.5. Versions of Xamarin Studio newer than
5.8.3 dropped support for Mac OS X 10.8. 

INTV.Intellicart.XamMac.csproj: Used for macOS
----------------------------------------------
This project was the initial port to Xamarin.Mac after it became freely
available. It represents the transition from MonoMac to the early unified
Xamarin.Mac platform, prior to Xamarin Studio being rebranded to Visual
Studio for Mac after Microsoft's acquisition of Xamarin. For all practical
purposes, this is identical to the Visual Studio for Mac project.

INTV.Intellicart.VSMac.csproj: Used for macOS
---------------------------------------------
Newer versions targeting 64-bit macOS are built using Visual Studio for Mac
and will only run in macOS 10.9 or later, and use this project.

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

DEPENDENCIES
=============================================================================
The INTV.Intellicart project depends on several other assemblies that are
part of the general VINTage umbrella. In addition to standard system
libraries, and MonoMac for the OS X build, INTV.Intellicart requires
the following:

INTV.Core
INTV.jzIntv
INTV.Shared

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - Commands: Intellicart-specific commands to configure a serial port to use
             for communication, and to download a game ROM to the device.

 - Model: Provides a software model for the Intellicart. This includes
          implementation of the protocol to download a ROM, as well as the
          necessary ROM conversion.

 - Properties: Contains assembly information and configurable settings.

 - Resources: Contains strings and images intended for display in the
              user interface.

 - View: This namespace contains implementations to provide a user interface
         to configure the serial port for use with an Intellicart.

 - ViewModel: This namespace contains the ViewModels used to communicate
              between the Intellicart model and any necessary user interface
              elements, such as configuration dialogs.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
