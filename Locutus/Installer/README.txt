=============================================================================
Installer Support Files
=============================================================================

This directory contains assets used to create the installers.

REQUIREMENTS
=============================================================================
The master images (*.psp) were created using an ancient version of Jasc
Software's Paint Shop Pro version 7.04. In the years since, it has been
purchased by Corel.

The GIMP was used to manipulate the background image used for the DMG
used to distribute the Mac OS X release. Image editing was performed in
Paint Shop Pro 7.04 in Windows.

The artwork used was created by Klay, and is a modified version of the
work used for the box and manual design.

BASIC_OVERVIEW
=============================================================================
The various installer_*.jpg files are used by the InstallShield LE 2013
scripts.

NOTE:
-----
In order to have InstallShield LE 2013 produce an installer with the desired
graphics, it was necessary to replace the setup.gif image in the installed
location. This file (setup.gif) must be placed in the following location:

  [Program Files (x86)]\InstallShield\2013LE\Support\Themes\InstallShield Blue Theme

The LTOFlashBackground-with-instructions.png file is used to provide the
background image for the disk image (DMG) used for Mac OS X distribution.
