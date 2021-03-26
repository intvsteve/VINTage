=============================================================================
LtoFlash
=============================================================================

The LtoFlash application is the user interface application that runs on
a desktop computer to assist you with managing your Intellivision (R) ROM
library and configure your LTO Flash! hardware for use.

HISTORY
=============================================================================
In February, 2014 development began in earnest on the LTO Flash! User
Interface Software. The basic framework for the software comes from a
lineage of other personal software projects undertaken to explore different
aspects of the C# programming language and MVVM development and dependency
injection techniques via the Managed Extensibility Framework (MEF).

The result, hopefully, is an easy-to-use desktop application for managing
not only your LTO Flash!, but your Intellivision ROM library as well.

REQUIREMENTS
=============================================================================
This application requires .NET 4.0 to run on Windows xp, and .NET 4.5 for
Windows Vista or later. Specific details about each version of the LtoFlash
project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

LtoFlash.desktop.csproj: Used for Windows Vista and later
---------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets.

LtoFlash.xp.csproj: Used for Windows xp
---------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

LtoFlash.Mac.csproj: Used for Mac OS X
--------------------------------------
This version of the project is used to build for 32-bit Mac OS X
deployments. It has been developed using MonoMac 4.2.1.102 and Xamarin
Studio 5.8.3. Good luck finding those old tools!

Development of the 32-bit Mac OS X version of the software was done on a
MacBook Pro running Mac OS X 10.8.5. Versions of Xamarin Studio newer than
5.8.3 dropped support for Mac OS X 10.8. 

LtoFlash.XamMac.csproj: Used for macOS
--------------------------------------
This project was the initial port to Xamarin.Mac after it became freely
available. It represents the transition from MonoMac to the early unified
Xamarin.Mac platform, prior to Xamarin Studio being rebranded to Visual
Studio for Mac after Microsoft's acquisition of Xamarin. For all practical
purposes, this is identical to the Visual Studio for Mac project.

LtoFlash.VSMac.csproj: Used for macOS
-------------------------------------
Newer versions targeting 64-bit macOS are built using Visual Studio for Mac
and will only run in macOS 10.9 or later, and use this project.

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

DEPENDENCIES
=============================================================================
The LtoFlash project's dependencies are mostly for convenience of the build
on the Windows platforms, while those on the Mac are are necessary due to
how MonoMac applications bundle (or fail to bundle) NIB resources into the
final application.

All platforms depend on the following:

INTV.Core
INTV.jzIntv
INTV.Shared
INTV.LtoFlash
INTV.Intellicart

In addition, there are some platform-specific dependencies which vary.

LtoFlash.xp:
------------
INTV.Ribbon

LtoFlash.Mac:
-------------
No further external dependencies beyond what is available with the tools.

LtoFlash.desktop:
-----------------
INTV.Ribbon

Two parts of the WindowsAPICodePack are used via INTV.Shared.desktop:

Core
Shell

LtoFlash was developed using the "official" 1.1 release, but the Microsoft
link for that download has since been retired. The most recent version is
available via a nuget package here:
  http://www.nuget.org/packages/winapicp/

BASIC OVERVIEW
=============================================================================
The LtoFlash project is a relatively thin application wrapper built around
the features provided by the assemblies it depends upon. What little
implementation there is as follows:

 - Properties: Contains application information.

 - Resources: The application resources.

 - View: The layout of the main application window resides here. This could
         be made more generic, and the level of generic-ness varies
         depending on the age of the code.

 - ViewModel: Contains the ViewModel for the main window. Ideally, all
              of the parts of the application would be discovered via MEF,
              but this is not the case. The ROM list from INTV.Shared and
              the LTO Flash component are still directly created.

 - Main: The main application is exposed slightly differently, depending on
         the target platform.

         Windows: The static Application class exposes the static Main entry
           point - a traditional WPF application.

         Mac: The MainClass exposes the static Main method, which is standard
           for MonoMac / Xamarin.Mac applications. Note, however, that the
           implementation has some (rather snarkily) commented dummy code
           present in order to coerce the build to produce the desired
           results. Due to the way the construction of visuals via NIBs is
           done, any C# assembly containing output from a XIB must be
           directly linked into the main application. And, in order to ensure
           that the NIB is accessible, a direct usage of any assembly
           containing said NIB must be made in the main application. Perhaps
           there are tricks to get this to work in another way. But for the
           time being, any assembly that ideally would be discovered and
           used entirely via MEF must, instead, be directly referenced by
           the main application. See the nonsense ****Hack code in Main.cs.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
