// <copyright file="OSExportAttribute.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Attribute for marking a method or property as 'exported'.
    /// </summary>
    /// <remarks>DO NOT confuse this with MEF exports! This is a compatibility shim class for
    /// working with Cocoa bindings on Mac, mostly. Doing this makes it easier to share code
    /// across platforms and reduce the number of redundant properties split out into partials.</remarks>
    [System.AttributeUsage(System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property)]
    public partial class OSExportAttribute
    {
    }
}
