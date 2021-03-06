﻿// <copyright file="AssemblyInfo.cs" company="INTV Funhouse">
// Copyright (c) 2013-2018 All Rights Reserved
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
[assembly: AssemblyTitle("Interface to jzIntv -- Portable Intellivision® Emulator")]
[assembly: AssemblyDescription("This class library defines models and utilities for working with jzIntv, Joe Zbiciak's Portable Intellivision® Emulator, as well as the SDK-1600. This assembly includes external utilities necessary to support features in jzIntv.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("INTVFunhouse")]
[assembly: AssemblyProduct("VINTage")]
[assembly: AssemblyCopyright("Copyright © INTV Funhouse 2013-" + INTV.VersionInfo.CurrentCopyrightYear)]
[assembly: AssemblyTrademark("Intellivision® is a registered trademark of Intellivision Entertainment, LLC. Steve Orth and INTV Funhouse, and Joe Zbiciak and Left Turn Only, LLC are not affiliated with Intellivision Entertainment, LLC.")]
//// Stupid xp... .NET 4.5 or later required for AssemblyMetadataAttribute
#if !WIN
[assembly: AssemblyMetadata(INTV.Core.Utility.ResourceHelpers.AuthorKey, "Steven A. Orth")]
#endif // !WIN
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e31b7508-5347-41d4-a860-4b54071c41f6")]

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
