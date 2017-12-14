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

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Bindings for methods that are either missing, or done incorrectly, for NSTableView.
    /// </summary>
    /// <remarks>TODO: Put into INTV.Shared to be generally available.</remarks>
    internal static class NSTableViewHelpers
    {
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_nint_nint_IntPtr_bool(System.IntPtr receiver, System.IntPtr selector, nint arg1, nint arg2, System.IntPtr arg3, bool arg4);
#endif // __UNIFIED__

        private static System.IntPtr selEditColumnRowWithEventSelectHandle = Selector.GetHandle("editColumn:row:withEvent:select:");

        /// <summary>
        /// Edits the cell at the specified column and row using the specified event and selection behavior.
        /// </summary>
        /// <param name="table">The <see cref="NSTableView"/> in which the cell exists.</param>
        /// <param name="column">The index of the column of the cell to edit.</param>
        /// <param name="row">The index of the row of the cell to edit.</param>
        /// <remarks>This doesn't seem to have a proper binding in MonoMac at this time. Perhaps newer versions of Xamarin.Mac have this binding. Either that, or the
        /// method fails because it won't allow passing <c>null</c> for the native method's <see cref="NSEvent"/> argument. The bindings seem to choke on that often.</remarks>
        internal static void EditColumn(this NSTableView table, nint column, nint row)
        {
            NSApplication.EnsureUIThread();
#if __UNIFIED__
            void_objc_msgSend_nint_nint_IntPtr_bool(table.Handle, selEditColumnRowWithEventSelectHandle, column, row, System.IntPtr.Zero, true);
#else
            Messaging.void_objc_msgSend_int_int_IntPtr_bool(table.Handle, selEditColumnRowWithEventSelectHandle, column, row, System.IntPtr.Zero, true);
#endif // __UNIFIED__
        }
    }
}
