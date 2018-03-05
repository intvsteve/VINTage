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
[assembly: AssemblyTitle("Interface library for LTO Flash!")]
[assembly: AssemblyDescription("This class library provides software models for working with the LTO Flash! product from Joe Zbiciak and Left Turn Only, LLC. It includes user interfaces for configuration and management of LTO Flash! devices.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("INTVFunhouse")]
[assembly: AssemblyProduct("VINTage")]
[assembly: AssemblyCopyright("Copyright © INTV Funhouse 2014-" + INTV.VersionInfo.CurrentCopyrightYear)]
[assembly: AssemblyTrademark("LTO Flash! is a product from Joe Zbiciak and Left Turn Only, LLC.")]
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
[assembly: Guid("8cd24362-4aa1-448e-be00-d56873751378")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion(INTV.VersionInfo.FullVersionString)]
