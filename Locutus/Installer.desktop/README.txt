=============================================================================
Installer.desktop
=============================================================================

This directory contains the InstallShield LE 2013 installer project for the
Windows Vista and newer installer.

REQUIREMENTS
=============================================================================
The InstallShield project Installer.desktop was created using InstallShield
Limited Edition 2013.

Image assets for the installer are located in the ../Installer directory
relative to this one.

To build the installer, the InstallShield Project must be included in the
same Visual Studio solution as the projects necessary to build the entire
LtoFlash.desktop.csproj project.

Target Platforms:
--------------------
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10

The installer produced by this project will only allow product installation
on Windows Vista or later.

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
  3) In the Installer.desktop project:
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
redistributable.

NOTE:
-----
In order to create an installer that will correctly support .NET 4.5
installation in Windows Vista, you may need to patch one of the prerequisites
files distributed with the InstallShield LE product. Specifically, you must
locate the following file:

[Program Files (x86)]\InstallShield\2013LE\SetupPrerequisites\Microsoft .NET Framework 4.5 Full.prq

  -- or --

[Program Files (x86)]\InstallShield\2015LE\SetupPrerequisites\Microsoft .NET Framework 4.5 Full.prq

In this file, you must add the following two lines, which apparently were
inadvertently omitted in the version of the file included with the product:

<operatingsystemcondition MajorVersion="6" MinorVersion="0" PlatformId="2" CSDVersion="" Bits="1"></operatingsystemcondition>
<operatingsystemcondition MajorVersion="6" MinorVersion="0" PlatformId="2" CSDVersion="" Bits="4"></operatingsystemcondition>

These lines must be added in the <operatingsystemconditions> node of the XML.

At issue is the omission of necessary data that the InstallShield system
uses to determine whether it is able to install a specific version of .NET.

DEPENDENCIES
=============================================================================
The integration with Visual Studio requires inclusion in a Visual Studio
solution that includes all of the projects necessary for the LtoFlash.desktop
project, which is located in the ../LtoFlash directory relative to this one.

Visual Studio solution files in the Locutus directory that are already
configured to produce an installer as part of the build process follow the
naming convention:

Locutus.desktop.installer[.*].sln

NOTE:
-----
The installer project should only be built from Release builds!
