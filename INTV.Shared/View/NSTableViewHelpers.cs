// <copyright file="NSTableViewHelpers.cs" company="INTV Funhouse">
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
//
#if __UNIFIED__
using AppKit;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

#if __UNIFIED__
using nint = System.nint;
#else
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Work around limitations in the MonoMac NSTableView bindings.
    /// </summary>
    internal static class NSTableViewHelpers
    {
        private static readonly System.IntPtr SelEditColumnRowWithEventSelectHandle = Selector.GetHandle("editColumn:row:withEvent:select:");
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_nint_nint_IntPtr_bool(System.IntPtr receiver, System.IntPtr selector, nint arg1, nint arg2, System.IntPtr arg3, bool arg4);
#endif // __UNIFIED__

        /// <summary>
        /// Start editing a cell in a given row and column. Yeah, the name is a little off.
        /// </summary>
        /// <param name="table">The <see cref=">NSTable"/> in which the edit operation is to occur.</param>
        /// <param name="column">The column number of the cell to edit.</param>
        /// <param name="row">The row number of the cell to edit.</param>
        internal static void EditColumn(this NSTableView table, nint column, nint row)
        {
            NSApplication.EnsureUIThread();
#if __UNIFIED__
            void_objc_msgSend_nint_nint_IntPtr_bool(table.Handle, SelEditColumnRowWithEventSelectHandle, column, row, System.IntPtr.Zero, true);
#else
            Messaging.void_objc_msgSend_int_int_IntPtr_bool(table.Handle, SelEditColumnRowWithEventSelectHandle, column, row, System.IntPtr.Zero, true);
#endif // __UNIFIED__
        }
    }
}
