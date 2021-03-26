=============================================================================
Installer.source
=============================================================================

This directory contains a GNU Makefile and a Windows batch file used to
create an archive of the source code for the LTO Flash! User Interface.

REQUIREMENTS
=============================================================================
An environment that supports the GNU 'make' utility (Mac) and a source
control system containing the code to archive. Currently supported tools
are SVN and GitHub.

In addition, the zip utility may be required.

Target Platforms:
--------------------
Mac OS X, Windows

DEPENDENCIES
=============================================================================
The integration with Visual Studio requires inclusion in a Visual Studio
solution that includes all of the projects necessary for the LtoFlash
project, which is located in the ../LtoFlash directory relative to this one.

Visual Studio solution files in the Locutus directory that are already
configured to produce an installer as part of the build process follow the
naming convention:

Locutus.*.installer[.*].sln

NOTE:
-----
The source archive project should only be built from Release builds!
