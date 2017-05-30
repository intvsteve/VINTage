// <copyright file="OSMessageBoxIcon.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific definition.
    /// </summary>
    public enum OSMessageBoxIcon
    {
        /// <summary>
        /// No icon displayed.
        /// </summary>
        /// <remarks>OK, well, actually, you cannot have 'none' so show info.</remarks>
        None = (int)NSAlertStyle.Informational,

        /// <summary>
        /// Question icon displayed.
        /// </summary>
        /// <remarks>Well... On Mac, it's a warning icon.</remarks>
        Question = (int)NSAlertStyle.Warning,

        /// <summary>
        /// Information icon displayed.
        /// </summary>
        Information = (int)NSAlertStyle.Informational,

        /// <summary>
        /// Exclamation icon displayed.
        /// </summary>
        Exclamation = (int)NSAlertStyle.Warning,

        /// <summary>
        /// Error icon displayed.
        /// </summary>
        Error = (int)NSAlertStyle.Critical,
    }
}
