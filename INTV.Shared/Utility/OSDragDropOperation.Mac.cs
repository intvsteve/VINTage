// <copyright file="OSDragDropOperation.Mac.cs" company="INTV Funhouse">
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

using System;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Abstraction for Cocoa drag-drop effects.
    /// </summary>
    [Flags]
    public enum OSDragDropOperations
    {
        /// <summary>
        /// A drag-drop target will not accept the data being dragged.
        /// </summary>
        None = (int)MonoMac.AppKit.NSDragOperation.None,

        /// <summary>
        /// Dragged data will be copied to drop target.
        /// </summary>
        Copy = (int)MonoMac.AppKit.NSDragOperation.Copy,

        /// <summary>
        /// Dragged data is linked to by the drop target.
        /// </summary>
        Link = (int)MonoMac.AppKit.NSDragOperation.Link,

        ////Generic = (int)MonoMac.AppKit.NSDragOperation.Generic,

        ////Private = (int)MonoMac.AppKit.NSDragOperation.Private,

        /// <summary>
        /// Dragged data is moved to the drop target.
        /// </summary>
        Move = (int)MonoMac.AppKit.NSDragOperation.Move,

        ////Delete = (int)MonoMac.AppKit.NSDragOperation.Delete,

        /// <summary>
        /// All operations apply.
        /// </summary>
        All = -1 // MonoMac.AppKit.NSDragOperation.All

    }
}
