=============================================================================
INTV.Shared
=============================================================================

The INTV.Shared assembly defines an Intellivision (R) ROM library manager as
well as a framework to which other extensions may be dropped in. It is
intended to act as the backbone for various Intellivision-related software
projects that fall under the general VINTage banner. (And it probably could
stand to be broken apart!)

HISTORY
=============================================================================
VINTage is the name coined for a mostly-completed, but never distributed,
Intellivision emulator implemented using the LabVIEW programming language
back around 2004. That project went dormant, but the notion of the VINTage
emulator never died.

This assembly began as part of an experiment to learn .NET 4.5 language
features by implementing a ROM library manager and new front end for the
jzIntv emulator from Joe Zbiciak.

Another reason for this project was to attempt to gain a better understanding
of the Managed Extensibility Framework - MEF. MEF defines a generic
mechanism by which software can be extended by declaring interface contracts,
and these contracts can be met at runtime. Additional software that declares
its available contracts can be discovered and added to the application
without re-deploying the entire application. (At least, that's the intent.)

Thus, you will find that INTV.Shared makes use of the MEF system to locate
plugins such as the Intellicart and LTO Flash! software without making
any direct references to them. Further extensions will hopefully be added
in future updates.

REQUIREMENTS
=============================================================================
This library requires .NET 4.0. Specific details about each version of
the INTV.Shared project are enumerated below.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10
Mac OS X 10.7 and later

INTV.Shared.desktop.csproj: Used for Windows Vista and later
-----------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets. It also requires two components from
the WindowsAPICodePack, the most recent version to be found here:
  http://www.nuget.org/packages/winapicp/

INTV.Shared.xp.csproj: Used for Windows xp
-----------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp.

INTV.Shared.Mac.csproj: Used for Mac OS X
----------------------------------------------
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

NOTE:
-----
The Mac version of this assembly contains a workaround for a bug in Mono
relating to the behavior of the Process.Start method, detailed here:

  https://bugzilla.xamarin.com/show_bug.cgi?id=37138

Code that runs external programs, whether or not it waits for the external
program to finish, will be checked to ensure the execute permission is set.
If not, an error will be reported, indicating to use chmod +x on the file
in question.

DEPENDENCIES
=============================================================================
The INTV.Shared project depends on other assemblies that are part of the
general VINTage umbrella. In addition to standard system libraries, and
MonoMac for the OS X build, INTV.Shared requires the following:

INTV.Core
INTV.jzIntv

In addition, there are some platform-specific dependencies which vary.

INTV.Shared.xp:
INTV.Shared.Mac:
----------------
No further external dependencies beyond what is available with the tools.

INTV.Shared.desktop:
--------------------
Two parts of the WindowsAPICodePack are used:

Core
Shell

INTV.Shared was developed using the "official" 1.1 release, but the Microsoft
link for that download has since been retired. The most recent version is
available via a nuget package here:
  http://www.nuget.org/packages/winapicp/

BASIC OVERVIEW
=============================================================================
This assembly provides the following general categories of services:

 - Behavior: In an attempt to hew rigorously to the MVVM architecture, many
             common tasks that are often implemented by subclassing WPF
             controls can be implemented more generally, and technically in
             a more architecturally correct fashion, by using the approach
             called "attached behaviors". The types in this namespace are
             such utilities, including mechanisms for creating adorners to
             edit text "in place," execute drag and drop operations via the
             RelayCommand mechanism, and other such features. This namespace
             is much heavier on the WPF side than the Cocoa (Mac) side.

 - Commands: Aside from defining the core interfaces and base implementation
             of the command system, the namespace also includes numerous
             commands used for ROM library management and standard
             application control. It also provides services to assist with
             the definition of data-driven command creation. This strategy is
             more successfully adopted in the Mac OS X implementation, as it
             started later than the initial WPF version. The command system
             could still use some improvements (e.g. the
             INTV.Shared.Commands.ICommandProvider interface can likely be
             merged with ICommandGroup).

 - ComponentModel: This namespace extends the concept of the basic
                   RelayCommand from INTV.Core.ComponentModel to include user
                   interface elements. It defines other application-level
                   interfaces, as well as some helper classes for working
                   with MEF - the Managed Extensibility Framework.

 - Converter: This namespace contains types used to convert data from the
              model for display in a user interface (View), via the ViewModel.

 - Interop: This namespace supplies implementations for lower-level functions
            not typically needed by most simple applications. Specifically,
            since it is known that applications using INTV.Shared are going
            to communicate with serial port-based hardware, such as the
            Intellicart, Cuttle Cart 3, and LTO Flash!, a common library of
            routines to report device arrival in the system is needed.

            For the WPF implementation, the simplicity and flexibility of the
            traditional Win32 WM_DEVICECHANGE message offered the cleanest
            and most reliable means to detect device arrival and departure
            in user-level code.

            For the Mac OS X implementation, two mechanisms have been
            developed. One is a simple file system monitor, which essentially
            watches for changes in the /dev/ directory. The other more
            "correct" implementation is implemented by developing bindings to
            the IOKit library. The MonoMac and Xamarin.Mac products do not
            provide "official" bindings to the IOKit, so that which was deemed
            necessary has been developed here.

            A third possible mechanism has been considered, but not developed.
            The IOKit has a command-line tool, which is poorly documented. It
            provides all the detail that the programmatic API does, presented
            as XML text, which could be parsed. As both the IOKit and file
            system watch mechanisms meet the needs of current applications,
            no further effort has been expended on this.

            Because the IOKit approach requires the use of p/Invoke -- it is
            not part of the Cocoa API and therefore C# bindings cannot be
            implemented using the Cocoa messaging APIs already available in
            MonoMac / Xamarin.Mac, it limits the possibility (currently) of
            moving to a free version of Xamarin.Mac.
 
 - Model: Provides helper functions for working with the IRom interface,
          user-specified configuration options, and an abstraction for
          stream-based device connections.

          The Device namespace supplies serial port- and named pipe-based
          implementations for the stream-based connection interface.

          The Program namespace provides a ROM collection manager and various
          support classes and extension methods for working with ROMs and
          their descriptive data. The largest set of classes define various
          "feature sets" used to describe compatibility and other
          requirements a ROM may have.

 - Properties: Contains assembly information and configurable settings.

 - Resources: Contains strings and images intended for display in the
              user interface.

 - Utility: Many general-purpose utility classes reside in this namespace.
            Some of the more important ones include:
            SingleInstanceApplication: a wrapper around the OS-specific
            application class
            IFileBrowserDialog: an interface used for file system dialogs
            Logger: used for gathering and reporting debug information
            AsyncTaskWithProgress: a workhorse for performing potentially
            long-running tasks in a thread pool, which will display a
            progress dialog as necessary

 - View: This namespace contains numerous user interface elements, such as
         controls to configure ROM features, a cross-platform message box,
         error report dialog, application settings dialog framework, and
         a ROM list presenter and editor.
         TODO: Split this to a separate assembly.

 - ViewModel: This namespace contains the ViewModels used to communicate
              between the various models and user interface elements.

OTHER
=============================================================================
Intellivision (R) is a registered trademark of Intellivision Entertainment.
Steven A. Orth and the software and products created from this code are not
affiliated with Intellivision Entertainment.
