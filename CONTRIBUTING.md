# Contributing to VINTage

Here is a set of [guidelines, not rules](https://www.youtube.com/watch?v=b6kgS_AwuH0), just in case you're brave enough to navigate this pile of code and wish to improve it.

#### Table Of Contents

[Code of Conduct](CODE_OF_CONDUCT.md)

[Getting started](#getting-started)

[How to Contribute](#how-to-contribute)
  * [Report Bugs](#report-bugs)
  * [Suggest Enhancements](#suggest-enhancements)
  * [Contribute Code](#contribute-code)
  * [Pull Requests](#pull-requests)

[Supported Platforms](#supported-platforms)

## Getting Started

### Development Tools

The VINTage projects are primarily written in C# and target the Big Three desktop platforms - Windows, macOS, and Linux. When you choose to contribute code, you should be sure to consider whether your changes will work on all of the [supported platforms](#supported-platforms). At this time, the following development tools are required:

* **Windows:** [Microsoft Visual Studio](https://www.visualstudio.com/downloads/) - ideally 2012 or later, though Visual Studio Express 2010 works - or can be made to work
  * **Installers:** At this time, the Windows installers are created with [InstallShield Limited Edition](https://docs.microsoft.com/en-us/visualstudio/deployment/deploying-applications-services-and-components)
  * **StyleCop** *(Optional):* StyleCop may be used to ensure comments are provided and consistent
  * **FxCop** *(Optional):* FxCop may be used to analyze the code and catch common errors and bad practices
* **Mac:** Solutions and projects exist for the following:
  * **MonoMac:** Use the \*.Mac.\*.sln solutions with Xamarin Studio 5.8.3 or later to build for 32-bit MonoMac
  * **Xamarin.Mac** Use the \*.XamMac.\*.sln solutions with newer versions of Xamarin Studio that support the UNIFIED build; *NOTE:* Consider these builds deprecated, because...
  * **Visual Studio for Mac:** Use the \*.VSMac.\*.sln solutions with [Visual Studio for Mac](https://www.visualstudio.com/vs/visual-studio-mac/) - the current offering for C# on Mac
  * **Installer:** A Disk Image (.dmg) is used to deliver the product, not a .pkg
* **Linux:** To target Linux, use [MonoDevelop](http://www.monodevelop.com/) and [GKTSharp](http://www.monodevelop.com/)

***NOTE:*** Because VINTage is still intended to run in Windows xp, the C# code is restricted to the features available in .NET 4.0.

***NOTE:*** The MonoMac build is 32-bit only. For 64-bit support, use the Visual Studio for Mac build. Eventually, MonoMac support will be retired. Until then, any new features must still be implemented in a manner such that the code will build with the older tools. The XamMac solution and projects were created prior to the acquistion of Xamarin by Microsoft. Those solutions and projects will also be retired eventually.

***Please*** do not attempt to submit solutions that are automatically updated by newer versions of the development environments! Sure, things are *supposed* to work with different versions. But we're a bit skeptical around here.

### Which Solution to Use

You will find numerous different solution (.sln) files to choose from. By examining the name of the solution file, you will be able to decide which one to use. The basic format of a solution file's name in VINTage is:

    <Name>.<Target>[.installer].<Source Control>.sln

The *\<Name\>* portion will be something like **Locutus** or **VINTage**.

The *\<Target\>* portion will be one of the following:
* ***Empty*** - Windows 7 and later (Visual Studio)
* **xp** - Windows xp (Visual Studio)
* **Mac** - MacOS X 10.7 and later (MonoMac, Xamarin Studio, possibly MonoDevelop, possibly Visual Studio for Mac)
* **XamMac** - MacOS X 10.7 and later (Xamarin.Mac, Xamarin Studio)
* **VSMac** - macOS 10.7 and later (Xamarin.Mac, Visual Studio for Mac)
* **Gtk** - Linux (Mono + GTKSharp, MonoDevelop) (presently only tested in Ubuntu 16.04.2)

The *.installer* solutions will include additional projects to create an installer for the executable. For macOS targets, the installer is a Disk Image (.dmg).

The *\<Source Control\>* portion of the name indicates the source control system used with the solution. The solutions here should all contain `.git` in the solution file name. Over the course of this project, there have been Team Foundation (tfs) and SVN (svn) solutions as well, though not all of these have been present in this repository.

Example:
The solution file `Locutus.xp.installer.git.sln` is a Visual Studio solution that will build the VINTage source and produce an installer for the Windows xp version of the software.

## How to Contribute

This sections describes the various ways you can contribute to the success of VINTage.

### Report Bugs

Don't be shocked but there *may* be bugs in this software! When you encounter one, visit the [issues](https://github.com/intvsteve/VINTage/issues) page to report it. Crashing bugs should create an error log in the `ErrorLogs` folder, even if you don't see the dialog that prompts to send an email directly to support. The issues page contains prompts to provide information about the problem. When reporting a bug, please include:

* The operating system version of the computer you are running the program on
* The version of the software you were using (from the **About** dialog)
* What the problem was
* What you expected to happen vs. what actually happened
* Label the issue as a `bug`

### Suggest Enhancements

If you have a feature request for VINTage, you can also use the [issues](https://github.com/intvsteve/VINTage/issues) page.

* Give a detailed description of what the new feature is
* Label the issue as an `enhancement`

### Contribute Code

TBD

#### Project File Naming Conventions

The C# project files (.csproj) in the solution use a naming convention to assist with identifying which solution the project file belongs to. These are as follows:
* `.pcl` - Indicates Portable Class Library, not supported in all solutions / for all targets
* `.xp` - Indicates the project builds for Windows xp in Visual Studio
* `.desktop` - Indicates the project builds for Windows 7 and later in Visual Studio
* `.Mac` - Indicates the project builds for MonoMac (32-bit) in Xamarin Studio
* `.XamMac` - Indicates the project builds for Xamarin.Mac in Xamarin Studio
* `.VSMac` - Indicates the project builds for Xamarin.Mac in Visual Studio for Mac
* `.Gtk` - Indicates the project builds for Mono and GTKSharp in MonoDevelop

There are additional, optional aspects to some projects that you may choose to explore. Specifically, the VINTage suite makes use of [jzIntv](http://spatula-city.org/~im14u2c/intv/) and utilities from the [SDK-1600](http://sdk-1600.spatula-city.org/). The jzIntv codebase is best compiled using [GCC 7.1](https://gcc.gnu.org/gcc-7/) or later. In addition, to build jzIntv and SDK-1600 in Windows, it is best done using [MinGW](http://mingw.org/). Some projects in the solutions are prepared to integrate with such a confguration.

#### File Naming Conventions

Generally, the coding convention is one class per file, with the name of the file matching the name of the class implemented in the file. If a class is a generic, then the file name expresses the generic types using a backtick character seprating the paramter types. For example:

    class Foo<T1, T2>

should be in a file named:

    Foo`T1`T2.cs

For situations in which portions of a class have platform-specific implementation needs, typically for View or ViewModel types, the platform-specific partial class files should be named beginning with the class name, followed by the platform, followed by the file extension. The standard platform-specific file name portions are:
* `.WPF` for Windows-specific code
* `.Mono` for Mono-specific (non-Windows / non-WPF) code, common to both macOS and Linux
* `.Mac` for macOS-specific code
* `.Gtk` for GTK-specific code
* `.Linux` for Linux-specific *non-user-interface* code

### Pull Requests

TBD

## Supported Platforms

VINTage is currently functional on the following platforms:

* Windows (this means Windows 7.0 and above)
* Mac (this means macOS 10.7 and above)
* Linux (this means... OK, I don't know what it means ... but a GTK version is under development!)
