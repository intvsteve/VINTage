=============================================================================
Installer.source
=============================================================================

This directory contains the InstallShield LE 2013 installer project for the
source code for the LTO Flash! User Interface.

REQUIREMENTS
=============================================================================
The InstallShield project Installer.source was created using InstallShield
Limited Edition 2013.

Image assets for the installer are located in the ../Installer directory
relative to this one.

To build the installer, the InstallShield Project must be included in the
same Visual Studio solution as the projects necessary to build the entire
LtoFlash project.

Target Platforms:
--------------------
Windows

The installer produced by this project will only allow product installation
on any version of Windows

DEPENDENCIES
=============================================================================
The integration with Visual Studio requires inclusion in a Visual Studio
solution that includes all of the projects necessary for the LtoFlash
project, which is located in the ../LtoFlash directory relative to this one.

Visual Studio solution files in the Locutus directory that are already
configured to produce an installer as part of the build process follow the
naming convention:

Locutus.*.installer[.*].sln
