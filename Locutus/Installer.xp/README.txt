=============================================================================
Installer.xp
=============================================================================

This directory contains the InstallShield LE 2013 installer project for the
Windows xp installer.

REQUIREMENTS
=============================================================================
The InstallShield project Installer.xp was created using InstallShield
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
run the installer produced by the Installer.desktop project.

NOTE:
-----
In order to have InstallShield LE 2013 produce an installer with the desired
graphics, it was necessary to replace the setup.gif image in the installed
location. This file (setup.gif) must be placed in the following location:

  [Program Files (x86)]\InstallShield\2013LE\Support\Themes\InstallShield Blue Theme

InstallShield LE 2015 exhibits the same behavior regarding the setup.gif
image. You must replace the file in the install location.

NOTE:
-----
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

For the Windows xp solutions, the installer requires the Microsoft .NET
Framework 4.0 Full redistributable.

!!!! NOTE NOTE NOTE !!!!
------------------------
In order to get a complete installer that includes jzIntv, you must do
the following:
  1. Ensure you have configured your system to build the jzIntv sources
     or place a copy of jzIntvUI in the appropriate location (see the
     README.txt for the INTV.jzIntv project)
  2. Build the INTV.jzIntvUI component BEFORE building everything
  3. THEN 'Rebuild Solution'

There's an odd quirk with how the InstallShield projects decide what is
'content' and what is not. The first time through, it won't pick up the
jzIntv files. Kludge city.

DEPENDENCIES
=============================================================================
The integration with Visual Studio requires inclusion in a Visual Studio
solution that includes all of the projects necessary for the LtoFlash.xp
project, which is located in the ../LtoFlash directory relative to this one.

Visual Studio solution files in the Locutus directory that are already
configured to produce an installer as part of the build process follow the
naming convention:

Locutus.xp.installer[.*].sln

NOTE:
-----
The installer project should only be built from Release builds!
