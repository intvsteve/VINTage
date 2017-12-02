 Contributing to VINTage

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

The VINTage projects are primarily written in C#. Therefore, when you choose to contribute code, you should be sure to consider whether your changes will work on all of the [supported platforms](#supported-platforms). At this time, the following development tools are required:

* **Windows:** [Microsoft Visual Studio](https://www.visualstudio.com/downloads/) - ideally 2012 or later, though Visual Studio Express 2010 works - or can be made to work
* **Mac:** Projects exist for the following:
  * **MonoMac:** Use the *.Mac.*.sln solutions with Xamarin Studio 5.8.3 or later to build for 32-bit MonoMac
  * **Xamarin.Mac** Use the *.XamMac.*.sln solutions with newer versions of Xamarin Studio that support the UNIFIED build; *NOTE:* Consider these builds deprecated, because...
  * **Visual Studio for Mac:** Use the *.VSMac.*.sln solutions with [Visual Studio for Mac](https://www.visualstudio.com/vs/visual-studio-mac/) - the current offering for C# on Mac
* **Linux:** To target Linux, use [MonoDevelop](http://www.monodevelop.com/) and [GKTSharp](http://www.monodevelop.com/)

***Please*** do not attempt to submit solutions that are automatically updated by newer versions of the development environments! Sure, things are *supposed* to work with different versions. But we're a bit skeptical around here.

There are additional, optional aspects to the project that you may choose to explore. Specifically, the VINTage suite makes use of [jzIntv](http://spatula-city.org/~im14u2c/intv/) and utilities from the [SDK-1600](http://sdk-1600.spatula-city.org/). 

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

### Pull Requests

TBD

## Supported Platforms

VINTage is currently functional on the following platforms:

* Windows (this means Windows 7.0 and above)
* Mac (this means macOS 10.7 and above)
* Linux (this means... OK, I don't know what it means ... but a GTK version is under development!)
