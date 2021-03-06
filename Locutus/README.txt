=============================================================================
Locutus
=============================================================================

This directory contains subdirectories for the LTO Flash! User Interface
application's executable project as well as various installers.

HISTORY
=============================================================================
In February, 2014 development began in earnest on the LTO Flash! User
Interface Software. Joe Zbiciak had already done most of the hardware
design for a project he had given the codename "Locutus". William Moeller
put Steve Orth back in touch with Joe to talk about creating a user interface
application for the project. One that would run on Mac and Windows.

As Joe bounced ideas off Steve's (admittedly at times quite thick) skull,
the file system design and general functionality and behavior of the firmware
and device solidified. In parallel with this, Steve exhumed some code that
had gone dormant about a year prior.

The basic framework for the software traces its lineage back through personal
software projects undertaken to explore different aspects of the C#
programming language, MVVM and WPF development. Since an integral part of
working with the LTO Flash! hardware involves working with a library of ROMs,
and a basic ROM library "manager" was something already developed in these
personal projects, it was a natural starting point.

The result, hopefully, is an easy-to-use desktop application for managing
not only your LTO Flash!, but your Intellivision (R) ROM library as well. In
the future, new additions to the software may bring additional functionality.

REQUIREMENTS
=============================================================================
The Windows application requires .NET 4.0 to run on Windows xp, and .NET 4.5
for Windows Vista or later. The Mac OS X release requires Mono 4.2. For the
Linux (GTK) version, Mono 5.0.1.1 or later is recommended.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 or newer
Linux ????

HOW TO BUILD
=============================================================================
Windows
-------
The Windows solutions for desktop (Windows Vista and later) require the
ability to build Portable Class Libraries. At the time this effort began,
Visual Studio 2012 Ultimate was the product used to create the projects and
solutions, and has remained so throughout development.

The Windows xp projects can be built with Microsoft Visual Studio Express
2010 -- if not directly, then with slight modification. With some effort,
specifically creating a 'desktop' version of the INTV.Core project and
updating the relevant dependencies in the other projects, a desktop target
could be built using Microsoft Visual Studio Express as well.

Newer versions of Visual Studio have not been tested. Perhaps support for
Portable Class Libraries is available in the free editions now, too.

You will also need a minimal MSYS2 setup in order to build the entire
solution. See the notes in the INTV.jzIntv and INTV.jzIntvUI projects.

macOS
-----
The Mac OS X projects have been built using Xamarin Studio 5.8.3 (as well as
some previous versions), and the 64-bit releases use Visual Studio for Mac.

The development environments should install .NET / Mono.

If you find you must install .NET 4.0 for xp development, visit this URL:
  https://www.microsoft.com/en-us/download/details.aspx?id=17851

For the .NET 4.5 installer:
  https://www.microsoft.com/en-us/download/details.aspx?id=42643

The Mac OS X application requires MonoMac to be installed. The current
release can be found here:
  http://www.mono-project.com/download/

Linux
-----
The Linux version is built using MonoDevelop and GtkSharp.
  See: http://www.mono-project.com/docs/gui/gtksharp/

To this point, development and testing for Linux has only been done using
Ubuntu 16.04.2 and explored on Arch Linux (2019.09.01).

 . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

You will find various *.sln (Visual Studio Solution) files in this directory.
These are intended to offer an all-in-one way to build the projects that
comprise the LTO Flash! User Interface Software. Depending on the tools
available to you, this could be easy -- or not. The solutions (*.sln) in
this directory have different requirements and offer different levels of
completeness.

The solutions and projects generally follow a simple naming convention:
  [Name].[TargetPlatform].csproj

Target platforms are:
---------------------
  Mac       : Mac OS X 10.7 (or 10.9) and newer
  xp        : Windows xp - restricted to .NET 4.0
  desktop   : Windows Vista and newer
  Linux/GTK : GTK2 ????
  pcl       : Windows Portable Class Library (currently .NET 4.0 era)

If no target is explicitly declared in a project's name, presume it is
targeted to Windows desktop.
_____________________________________________________________________________
  Locutus.svn.sln | Visual Studio 2012 solution to build application and
  Locutus.git.sln | support libraries; if building source for SDK-1600
                  | and/or jzIntv, you will need to specify their location;
                  | requires .NET 4.5 and the ability to build
                  | Portable Class Libraries; targets Windows Vista+
  -.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-
  Locutus.installer.svn.sln | Same as Locutus.svn.sln, but includes projects
  Locutus.installer.git.sln | for InstallShield LE 2013 installers
 ---------------------------------------------------------------------------
  Locutus.xp.svn.sln | Visual Studio 2012 solution to build application and
  Locutus.xp.git.sln | support libraries; if building source for SDK-1600
                     | and/or jzIntv, you will need to specify their
                     | location;requires .NET 4.0; targets Windows xp
  -.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-
  Locutus.xp.installer.svn.sln | Same as Locutus.xp.svn.sln, but includes
  Locutus.xp.installer.git.sln | projects for InstallShield LE 2013 installers
 ---------------------------------------------------------------------------
  Locutus.Mac.svn.sln | Xamarin Studio 5.8.3 solution to build application and
  Locutus.Mac.git.sln | support libraries; if building source for SDK-1600
                      | and/or jzIntv, you will need to specify their location;
                      | requires MonoMac 4.2; targets Mac OS X 10.7 and newer;
                      | NOTE: 32-bit only!
  Locutus.XamMac.svn.sln | Xamarin Studio 6.3 solution to build application
  Locutus.XamMac.git.sln | and support libraries; if building source for
                         | SDK-1600 and/or jzIntv, you will need to specify
                         | their location; requires Xamarin.Mac; targets
                         | Mac OS X 10.9 and newer; will be retired due to
                         | Visual Studio for Mac replacing Xamarin Studio
                         | NOTE: 64-bit only!
  Locutus.VSMac.svn.sln | Visual Studio for Mac 8.0.6 solution to build
  Locutus.VSMac.git.sln | application and support libraries; if building
                        | source for SDK-1600 and/or jzIntv, you will need to
                        | specify their location; requires Xamarin.Mac;
                        | targets Mac OS X 10.9 and newer
                        | NOTE: 64-bit only!
  -.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-
  Locutus.Mac.installer.svn.sln | Same as Locutus.Mac.svn.sln, but includes
  Locutus.Mac.installer.git.sln | projects for installers
 ---------------------------------------------------------------------------
  Locutus.Gtk.svn.sln | MonoDevelop 5.10 solution to build application and
  Locutus.Gtk.git.sln | support libraries; if building source for SDK-1600
                      | and/or jzIntv, you will need to specify their location
_____________________________________________________________________________

The components necessary to build the LTO Flash! User Interface Software are:
___________________________________________________________________________
| Component          | Role
---------------------------------------------------------------------------
| INTV.Core          | Basic models of ROMs, ROM database, et. al.
| INTV.jzIntv        | Access to utilities from jzIntv / SDK-1600
| INTV.LtoFlash      | Models and communication library for LTO Flash!
| INTV.Ribbon        | Abstraction of Microsoft Ribbon API for .NET 4.0/4.5
| INTV.Shared        | ROM library management and general utilities
| LtoFlash           | The actual application program
---------------------------------------------------------------------------
| WindowsAPICodePack | .NET 4.5-specific library used by desktop builds
---------------------------------------------------------------------------

At this time, the following "expansion" features have also been added:
  1. INTV.Intellicart
  2. INTV.jzIntvUI

Future expansion features may include INTV.CuttleCart3 and other yet-to-be
defined capabilities. It is not necessary to build the Intellicart or
jzIntvUI features in order to create an application to work with LTO Flash!
hardware.

Each project directory contains a README.txt file (or an obvious analog)
with information specific to the project within that directory.

Installer Projects
-----------------------------------------------------------------------------
All Windows installer projects were created using InstallShield Limited
Edition 2013. Consult the notes in the various Installer.* subdirectories.
For any Visual Studio solution file including the '.installer' indication,
InstallShield LE 2013 should be present on the system.

InstallShield LE 2013 Issues:
-----------------------------
Some modifications to the installed version of InstallShield LE 2013 are
necessary to produce the desired results. Consult the README.txt files in
the various installer project directories for further details.

Mac OS X Installers:
--------------------
The 32-bit software for Mac OS X is distributed via a Disk Image (DMG).
The DMG is produced via a GNU makefile.

At present, the process to produce a fancy DMG for the 64-bit macOS builds
has not been defined. The blank disk image is not at present large enough,
and the process of creating and automating its creation has not been
undertaken. At this time, simple ZIP distribution without additional
"installation" instructions is used.

Source Code Distribution:
-------------------------
The simplest source code distribution is accomplished via a ZIP file, which
can be created via a GNU makefile.

Other Build Notes and Questions:
--------------------------------
QUESTION: Why not just stick with .NET 4.0?
-------------------------------------------
This certainly would have made certain things easier. However, the .NET 4.0
Microsoft Ribbon library looks dated on modern operating systems. With a
relatively small amount of effort (the INTV.Ribbon component) and a bit more
diligence in maintaining projects, the tradeoff is worth it.

QUESTION: Why are there three Mac projects and solutions?
---------------------------------------------------------
When this project started, Xamarin.Mac was not available for projects such
as this one in a form that supported the features it needed. Another reason
was the cost of a seat that would support p/Invoke. At the time of initial
development and release, the Mac version of the software had to use the
MonoMac libraries, rather than Xamarin.Mac, to remain free. Also, as a
consequence of this, the Mac software from that build runs as a 32-bit
application.

In subsequent years, Microsoft purchased Xamarin, Xamarin.Mac became free,
created the unified API, and support for 64-bit applications. Eventually
Microsoft rebranded Xamarin Studio to be Visual Studio for Mac. With each
of these epochs, parallel projects and solutions were created to ensure
functioning projects for "known good" tools stayed available. Through all
this, the mission to continue supporting macOS as far back as 10.7 has
persisted. This is still the case, despite the fact that Visual Studio for
Mac (and prior, Xamarin Studio) and the Xamarin.Mac platform no longer
allow targeting anything prior to macOS 10.9.

Further, Apple eventually killed off 32-bit application support in an OS
update. Thus, at least two different parallel solutions must exist, as
long as macOS prior to 10.9 is still supported. The ancient and venerable
machine still running a woefully outdated release of Xamarin Studio in
macOS 10.8.5 is keeping this alive.

QUESTION: Who do I talk to about this pile of code?
---------------------------------------------------
Contact support@intvfunhouse.com with any questions.

EXTERNAL DEPENDENCIES
=============================================================================
In the Windows desktop builds, two parts of the WindowsAPICodePack are used:

Core
Shell

This project was developed using the "official" 1.1 release, but the Microsoft
link for that download has since been retired. This source distribution
contains the code from that release, as it may be freely distributed.
The most recent version is available via a nuget package here:

  http://www.nuget.org/packages/winapicp/

Other distributions may be out in the great wide world, too.

All versions also consume the ICSharpCode.SharpZipLib, whose official home
can be found on GitHub:

  https://github.com/icsharpcode/SharpZipLib

Because this project targets multiple platforms AND as a bonus, super old
.NET 4.0, AND uses old tools, by necessity a back-ported clone of the source
for ICSharpZipLib is included in this repo, and projects to build on Mac and
Linux are provided.

When this project is modernized to use .NET 5 or later and Windows xp
support is dropped, these external dependencies will be removed.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
