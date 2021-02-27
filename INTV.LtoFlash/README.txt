=============================================================================
INTV.LtoFlash
=============================================================================

The INTV.LtoFlash assembly defines features to use the LTO Flash! cartridge
from Left Turn Only, LLC. It is designed to provide such functionality
to various Intellivision (R)-related software projects that fall under the
general VINTage banner.

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

The effort to develop a modern, easy-to-use user interface for a modern,
easy-to-use flash memory-based cartridge emulator, the LTO Flash!, started
in February 2014. Since production of the Chad Schell's Micro SD card-based
Cuttle Cart 3 stopped back in 2007, and the format of SD cards it supports
has dropped out of the market, no new alternative has been available.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.LtoFlash project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.LtoFlash.desktop.csproj: Used for Windows Vista and later
------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets.

INTV.LtoFlash.xp.csproj: Used for Windows xp
------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.LtoFlash.Mac.csproj: Used for Mac OS X
-----------------------------------------
This version of the project is used to build for Mac OS X deployments. It
has been developed using MonoMac 4.2.1.102 and Xamarin Studio 5.8.3. You
can find MonoMac at: http://www.mono-project.com/download/ and Xamarin
Studio at: http://xamarin.com/download

Development of the Mac OS X version of the software was done on a MacBook
Pro running Mac OS X 10.8.5. Versions of Xamarin Studio newer than 5.8.3
dropped support for Mac OS X 10.8. It is unknown if there are compatibility
issues with this project in newer versions of Xamarin Studio.

NOTE:
-----
Because the Mac OS X projects make use of p/Invoke to call native
system libraries, at this time the Mac version of the software must use
the MonoMac libraries, rather than Xamarin.Mac, to remain free. The standard
free version of Xamarin.Mac does not support p/Invoke. Also, as a
consequence of this, the Mac software runs as a 32-bit application. This may
change in the future.

NOTE:
-----
With Microsoft's acquisition of Xamarin in March of 2016, the opportunity
to port the Mac version to full Xamarin.Mac now exists!

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

DEPENDENCIES
=============================================================================
The INTV.LtoFlash project depends on several other assemblies that are
part of the general VINTage umbrella. In addition to standard system
libraries, and MonoMac for the OS X build, INTV.LtoFlash requires
the following:

INTV.Core
INTV.jzIntv
INTV.Shared

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - Commands: Defines LTO Flash!-specific user-facing commands to configure
             and communicate with the LTO Flash! hardware. The commands are
             usually invoked from the user interface to perform specific
             actions, such as to manage the file system on LTO Flash!, change
             compatibility modes, et. al. Debug builds offer additional
             "secret" commands to exercise certain features in isolation.

 - Converter: This namespace contains types used to convert data from the
              model for display in a user interface (View), via the ViewModel.

 - Model: Provides a software model to represent the LTO Flash! hardware, the
          file system it uses (LFS), and serial communication protocol. To
          keep a responsive user interface, all serial communication is done
          using asynchronous tasks, which use a thread pool. This model can
          also seamlessly communicate with locutus_sim, the software emulator
          of the LTO Flash! hardware, developed by Joe Zbiciak.

          Individual serial protocol level commands, implemented in the
          Model.Commands namespace, are pooled together to form higher-
          level user-digestible tasks, such as a file system update, via
          the INTV.Shared.Utility.AsyncTaskWithProgress along with the
          INTV.LtoFlash.Model.ExecuteDeviceCommandAsyncTaskData type.
          User-interface driven commands specified in the Commands namespace
          ultimately are performed by the implementation in the DeviceHelpers
          class. INTV.LtoFlash.Model.DeviceHelpers provides the actual
          command execution of those user-facing commands.

          The Locutus File System (LFS) is modeled using the GlobalForkTable,
          GlobalFileTable, and GlobalDirectoryTable, along with interfaces
          and types representing forks, files, and directories. A specific
          subclass of the directory type, INTV.LtoFlash.Model.MenuLayout,
          represents the root of a LFS instance. These elements are collected
          in the FileSystem class, which coordinates these tables. These
          types enforce the behaviors specified by the LFS specification.

          In addition to the file system models (for both the user-facing
          version presented in the user interface program, and the actual
          residing on LTO Flash! itself), the Model namespace provides
          access to the error log and crash log implemented in the device
          firmware, as well as facilities to update firmware, and more.

 - Properties: Contains assembly information and configurable settings.

 - Resources: Contains strings and images intended for display in the
              user interface.

 - redist: This directory contains a snapshot of the FTDI serial port driver
           software for each platform. It should be used only as a last
           resort. Modern operating systems should automatically install the
           most recent driver software upon initial connection to LTO Flash!.

 - Utility: Contains utility code relating to the FTDI driver software.

 - View: The user interface elements for LTO Flash! are implemented by the
         classes in this namespace. These include configuration dialogs and
         user settings pages.
         TODO: Split this to a separate assembly.

 - ViewModel: The ViewModel namespace bridges between the Model and View
              namespaces, presenting access to the Model in a fashion more
              amenable to user interface actions and data types. The three
              primary ViewModels are:

              DeviceViewModel, representing a specific instance of the
              LTO Flash! hardware connected to a computer described by
              an instance of INTV.LtoFlash.Model.Device

              MenuLayoutViewModel, which represents an instance of the LFS
              file system described by INTV.LtoFlash.Model.MenuLayout

              LtoFlashViewModel, which represents a collection of LTO Flash!
              devices present in the system, as well as the instance of LFS
              that the user edits in the user interface
              NOTE: This may undergo somewhat radical changes if significant
              changes to the multi-device user interface experience are made

FURTHER WORK
=============================================================================
During development, a small effort was put in to support decoding the error
log produced by LTO Flash to map the encoded message back to information
defined in the specific version of the firmware running on the device. To
do this, a YAML parser is necessary. This work was begun, but discontinued
as the firmware - and file system - have proven highly reliable. That is,
the feature was deemed unnecessary.

The most interesting and compelling future work is more extensive, and is
to improve and augment the situation in which multiple LTO Flash! devices
are in use simultaneously. Visually, instead of a single menu, the editor
will expand to be tree containing the present menu layout as one node, with
each LTO Flash! in the system present as another node at the same level.
Copying data among the various sources and destinations would be accomplished
by dragging and dropping among the various nodes in this tree.

A simpler improvement will be to allow multiple item selection in the
Menu Layout editor.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
