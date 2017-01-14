=============================================================================
Installer Support Files
=============================================================================

This directory contains assets used to create the installers. as well as
a Visual Studio makefile project to place setup.exe installers into a
.zip file for distribution.

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

To build the Visual Studio makefile project, the current Installer.vcxproj
assumes you have configured MinGW / MSYS on your system, along with
ensuring that the zip utility is installed. Details of that configuration
are in the install_zip.bat and install_zip.mak files.

BASIC_OVERVIEW
=============================================================================
The various installer_*.jpg files are used by the InstallShield LE 2013
scripts.

The batch and makefiles are used to place setup.exe into an appropriately
named .zip file for simpler distribution.

NOTE (Windows InstallShield LE Installer Projects):
---------------------------------------------------
In order to have InstallShield LE 2013 produce an installer with the desired
graphics, it was necessary to replace the setup.gif image in the installed
location. This file (setup.gif) must be placed in the following location:

  [Program Files (x86)]\InstallShield\2013LE\Support\Themes\InstallShield Blue Theme

InstallShield LE 2015 exhibits the same behavior regarding the setup.gif
image. You must replace the file in the install location.

NOTE (Windows InstallShield LE Installer Projects):
---------------------------------------------------
In order to get the required .NET redistributables for the InstallShield
installers, you must, after installing InstallShield LE 20xx:
  1) Launch Visual Studio *as administrator*
  2) Open a solution that includes the Installer projects
  3) In the Installer.desktop or Installer.xp project:
    a) Open Section 2: Specify Application Data
    b) Double-click on the Redistributables item
    c) For each item that is checked, right-click and choose:
         'Download Selected Item...'
           -- or --
       choose (once?):
         'Download All Required Items...'
       This step ensures that the necessary redistributable files, such as
       the required .NET installers, are available.

For the Windows Vista and later projects (the .desktop solutions), the
installer will require the Microsoft .NET Framework 4.5 Full
redistributable. For the Windows xp solutions, the installer requires the
Microsoft .NET Framework 4.0 Full redistributable.

NOTE (Mac OS X Disk Image (.dmg) Installer):
--------------------------------------------
The LTOFlashBackground-with-instructions.png file is used to provide the
background image for the disk image (DMG) used for Mac OS X distribution.
