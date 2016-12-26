=============================================================================
Installer.xp.source
=============================================================================

This directory contains the InstallShield LE 2013 installer project for the
Windows xp installer, which includes source code.

REQUIREMENTS
=============================================================================
The InstallShield project Installer.xp.source was created using InstallShield
Limited Edition 2013.

Image assets for the installer are located in the ../Installer directory
relative to this one.

To build the installer, the InstallShield Project must be included in the
same Visual Studio solution as the projects necessary to build the entire
LtoFlash.xp.csproj project.

Target Platforms:
--------------------
Windows xp

The installer can be run on later versions of Windows. However, it is
strongly recommended that newer operating systems (Windows Vista and later)
run the installer produced by the Installer.desktop.source project.

DEPENDENCIES
=============================================================================
The integration with Visual Studio requires inclusion in a Visual Studio
solution that includes all of the projects necessary for the LtoFlash.xp
project, which is located in the ../LtoFlash directory relative to this one.

Visual Studio solution files in the Locutus directory that are already
configured to produce an installer as part of the build process follow the
naming convention:

Locutus.xp.installer[.*].sln
