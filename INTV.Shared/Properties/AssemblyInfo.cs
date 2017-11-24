// <copyright file="AssemblyInfo.cs" company="INTV Funhouse">
// Copyright (c) 2013-2017 All Rights Reserved
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
[assembly: AssemblyTitle("Intellivision® ROM library manager")]
[assembly: AssemblyDescription("This class library provides software models describing Intellivision® ROMs and a user interface for managing a ROM library, as well as platform abstractions for related user interface development.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("INTVFunhouse")]
[assembly: AssemblyProduct("VINTage")]
[assembly: AssemblyCopyright("Copyright © INTV Funhouse 2013-2017")]
[assembly: AssemblyTrademark("Intellivision® is a registered trademark of Intellivision Productions. Steve Orth and INTV Funhouse are not affiliated with Intellivision Productions.")]
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
[assembly: Guid("1568a04e-5e73-46ae-b35a-bf8ff2942229")]

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
[assembly: AssemblyFileVersion("1.0.0.0")]
