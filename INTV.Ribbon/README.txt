=============================================================================
INTV.Ribbon
=============================================================================

The INTV.Ribbon assembly defines a simple wrapper layer so user interfaces
built upon the Microsoft Ribbon libraries can be built using either .NET 4.0
or .NET 4.5.

HISTORY
=============================================================================
This assembly was developed when it was determined that the LTO Flash! User
Interface Software would be supported in Windows xp. Because the software
uses the Microsoft Ribbon API, this library became necessary due to the
differences between the .NET 4.0 and .NET 4.5 versions. As such, it presents
a "Least Common Denominator" between the two versions. It has also been
developed ad-hoc, not as an exhaustive mapping of the entire Ribbon API.

REQUIREMENTS
=============================================================================
The Windows xp version of this library requires .NET 4.0 and the
RibbonControlsLibrary, as documented at:
  https://msdn.microsoft.com/en-us/library/ff799534(v=vs.100).aspx

This may be downloaded from:
  https://www.microsoft.com/en-us/download/details.aspx?id=11877

The modern (Windows Vista and later) version of this does not need
additional software, as it is part of .NET 4.5 natively.

Supported Platforms:
--------------------
Windows xp
Windows Vista, Windows 7, Windows 8, , Windows 8.1, Windows 10

INTV.Ribbon.desktop.csproj: Used for Windows Vista and later
------------------------------------------------------------
This version of the project is used to build for Windows Vista and newer
Windows operating system targets. It must be built using .NET 4.5 or later.

INTV.Ribbon.xp.csproj: Used for Windows xp
------------------------------------------
This version of the project is used to build for Windows xp. It may be
built using Microsoft Visual Studio Express 2010 and newer. This version
cannot support any version of .NET later than 4.0, which was the final
.NET release supported on Windows xp. In order to build this version of
the project, you must install the Microsoft Ribbon for WPF October 2010
library, available here:

  https://www.microsoft.com/en-us/download/details.aspx?id=11877

NOTE:
-----
The Windows builds presume using a version of Microsoft Visual Studio
that can run the StyleCop and FxCop utilities for source and code analysis.
These features can be disabled with appropriate modifications to the projects.

DEPENDENCIES
=============================================================================
The INTV.Ribbon.xp project depends on one other assembly that must be
downloaded and installed separately:

  RibbonControlsLibrary (see details above)

The INTV.Ribbon.desktop project has no external requirements.

BASIC OVERVIEW
=============================================================================
This assembly provides a wrapper layer around the WPF Ribbon API. When
developing an application that uses the WPF Ribbon that must support
Windows xp as well as later operating systems, use this library. Applications
targeted for Windows Vista and later will look and behave better when using
the .NET 4.5-based version of this library (INTV.Ribbon.desktop).
