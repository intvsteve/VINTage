// <copyright file="AssemblyInfo.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LTO Flash! User Interface")]
[assembly: AssemblyDescription("This is a user interface application for the LTO Flash! product from Joe Zbiciak and Left Turn Only, LLC. It includes an Intellivision® ROM library manager as well as facilities to manage the behavior and content of LTO Flash! devices.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("INTVFunhouse")]
[assembly: AssemblyProduct("LTOFlash")]
[assembly: AssemblyCopyright("Copyright © INTV Funhouse 2014-" + INTV.VersionInfo.CurrentCopyrightYear)]
[assembly: AssemblyTrademark("Intellivision® is a registered trademark of Intellivision Entertainment, LLC. LTO Flash! is a product from Left Turn Only, LLC. Steve Orth and INTV Funhouse, and Joe Zbiciak and Left Turn Only, LLC are not affiliated with Intellivision Entertainment, LLC.")]
//// Stupid xp... .NET 4.5 or later required for AssemblyMetadataAttribute
#if !WIN_XP
[assembly: AssemblyMetadata(INTV.Core.Utility.ResourceHelpers.AuthorKey, "Steven A. Orth")]
[assembly: AssemblyMetadata(INTV.Core.Utility.ResourceHelpers.AboutKey, "Intellivision® ROM library manager and user interface for the LTO Flash! cartridge from Left Turn Only, LLC.")]
[assembly: AssemblyMetadata(INTV.Core.Utility.ResourceHelpers.WebsiteKey, "http://www.intvfunhouse.com/intvfunhouse/ltoflash/")]
[assembly: AssemblyMetadata(INTV.Core.Utility.ResourceHelpers.WebsiteNameKey, "INTV Funhouse")]
#endif // !WIN
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// In order to begin building localizable applications, set 
// <UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
// inside a <PropertyGroup>.  For example, if you are using US English
// in your source files, set the <UICulture> to en-US.  Then uncomment
// the NeutralResourceLanguage attribute below.  Update the "en-US" in
// the line below to match the UICulture setting in the project file.

////[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

#if WIN
[assembly: System.Windows.ThemeInfo(
    System.Windows.ResourceDictionaryLocation.None, // where theme specific resource dictionaries are located
    // (used if a resource is not found in the page, 
    // or application resource dictionaries)
    System.Windows.ResourceDictionaryLocation.SourceAssembly // where the generic resource dictionary is located
    // (used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
#endif // WIN

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion(INTV.VersionInfo.FullVersionString)]
[assembly: AssemblyInformationalVersion(INTV.VersionInfo.ShortVersionString + " " + INTV.VersionInfo.BuildVersionString)]
