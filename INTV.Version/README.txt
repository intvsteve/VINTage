=============================================================================
INTV.Version
=============================================================================

The INTV.Version is a dummy project that is used to update assembly version
values for the INTV.Core and other projects. It generates a C# source file
named 'VersionInfo.cs' in the root directory. The file is used by all of the
other assemblies (aside from externally sourced code) to define parts of the
AssemblyInfo.cs files that are part any C#-sourced .NET assembly used in
Intellivision (R)-related projects that fall under the VINTage banner.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

REQUIREMENTS
=============================================================================
This project invokes GNU make. On Windows systems, this means (most likely)
having MSYS2 configured on your system. For automated updating of the version
number, configuration of an SVN or Git repo is required as well. Detailed
notes are in the various makefiles and batch files at the root of the code
repository.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later
Linux (various)

INTV.Version.Mono.mdproj: Used for Mac and Linux.
-----------------------------------------------------
Easy peasy. Runs the version_tools.mak.

INTV.Version.Win.vcxproj: Used for Windows
----------------------------------------------
Runs version_tools.bat, which then runs version_tools.mak using
the bash provided with MSYS2.

PREREQUISITES
=============================================================================
SVN:
--------
The automated "true" version updating requires the SVN repo. Sorry, but
that one is not public.

Git:
--------
Maybe someday something will be worked out for Git, but until then, the best
that the makefile can do is tweak the 'number of modified files' part of the
version strings.

Mac / Linux:
----------------
GNU make is needed.

Windows:
------------
To this point, the makefile has only been verified working using GNU make
run in the bash provided with MSYS2.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
