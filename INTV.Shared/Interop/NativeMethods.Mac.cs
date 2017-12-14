// <copyright file="NativeMethods.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#if __UNIFIED__
using CoreFoundation;
using Foundation;
#else
using MonoMac.CoreFoundation;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Interop
{
    /// <summary>
    /// Provides access to Mac-specific methods that allow access to adding and removing CFRunLoopSource objects
    /// obtained directly from other APIs not bound to by the MonoMac framework.
    /// </summary>
    public static partial class NativeMethods
    {
        /// <summary>
        /// Adds a CFRunLoopSource. Necessary because we can't directly construct CFRunLoopSource from native IntPtr values in MonoMac.
        /// </summary>
        /// <param name="loop">The CFRunLoop to which the source is to be added.</param>
        /// <param name="source">The CFRunLoopSource to add.</param>
        /// <param name="mode">The mode for the run loop.</param>
        public static void CFRunLoopAddSource(CFRunLoop loop, System.IntPtr source, NSString mode)
        {
            if (mode == null)
            {
                throw new System.ArgumentNullException("mode");
            }

            CFRunLoopAddSource(loop.Handle, source, mode.Handle);
        }

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        private static extern void CFRunLoopAddSource(System.IntPtr loop, System.IntPtr source, System.IntPtr mode);

        /// <summary>
        /// Removes a CFRunLoopSource. Necessary because we can't directly construct CFRunLoopSource from native IntPtr values in MonoMac.
        /// </summary>
        /// <param name="loop">The CFRunLoop from which the source is to be removed.</param>
        /// <param name="source">The CFRunLoopSource to remove.</param>
        /// <param name="mode">The mode for the run loop.</param>
        /// <returns><c>true</c> if the CFRunLoopSource was removed.</returns>
        public static bool CFRunLoopRemoveSource(CFRunLoop loop, System.IntPtr source, NSString mode)
        {
            if (mode == null)
            {
                throw new System.ArgumentNullException("mode");
            }
            return CFRunLoopRemoveSource(loop.Handle, source, mode.Handle);
        }

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        private static extern bool CFRunLoopRemoveSource(System.IntPtr loop, System.IntPtr source, System.IntPtr mode);
   }
}
