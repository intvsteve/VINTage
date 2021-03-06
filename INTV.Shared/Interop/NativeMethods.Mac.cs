﻿// <copyright file="NativeMethods.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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

using Constants = ObjCRuntime.Constants;
#else
using MonoMac.CoreFoundation;
using MonoMac.Foundation;

using Constants = MonoMac.Constants;
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
        /// Use with <see cref="GetSystemProperty(string)"/> to get total memory in the system.
        /// </summary>
        public const string SystemRamSize = "hw.memsize";

        /// <summary>
        /// Use with <see cref="GetSystemProperty(string)"/> to get the model ID of the system.
        /// </summary>
        public const string SystemModel = "hw.model";

        /// <summary>
        /// Use with <see cref="GetSystemProperty(string)"/> to get processor architecture in the system.
        /// </summary>
        public const string SystemMachine = "hw.machine";

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

        [DllImport(Constants.CoreFoundationLibrary)]
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

        /// <summary>
        /// Gets a system property by name, returning the value as a string.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The system property to get.</returns>
        /// <remarks>This is a brain-dead bespoke implementation not suitable for general use. It has special
        /// behaviors and really only supports the properties defined by this class.
        /// Inspired by https://forums.xamarin.com/discussion/20006/access-to-sysctl-h </remarks>
        public static string GetSystemProperty(string propertyName)
        {
            string propertyValue = null;
            var lengthPointer = System.IntPtr.Zero;
            var error = sysctlbyname(propertyName, null, ref lengthPointer, null, 0);
            if (error == 0)
            {
                var length = lengthPointer.ToInt32();
                var propertyValueBuffer = new byte[length];
                error = sysctlbyname(propertyName, propertyValueBuffer, ref lengthPointer, null, 0);
                if (error == 0)
                {
                    switch (propertyName)
                    {
                        case SystemRamSize:
                            if (length == sizeof(long))
                            {
                                var memSize = System.BitConverter.ToInt64(propertyValueBuffer, 0);
                                decimal memGigabytes = memSize / 1024 / 1024 / 1024;
                                propertyValue = memGigabytes.ToString(System.Globalization.CultureInfo.InvariantCulture) + " GB";
                            }
                            break;
                        default:
                            // Don't need NULL terminator.
                            propertyValue = System.Text.Encoding.ASCII.GetString(propertyValueBuffer, 0, length - 1);
                            break;
                    }
                }
            }

            return propertyValue;
        }

        [DllImport(Constants.CoreFoundationLibrary)]
        private static extern bool CFRunLoopRemoveSource(System.IntPtr loop, System.IntPtr source, System.IntPtr mode);

        /// <summary>
        /// Invokes the sysctlbyname function.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        /// <param name="output">Current value output pointer.</param>
        /// <param name="oldLen">Current value length pointer.</param>
        /// <param name="newp">New value pointer.</param>
        /// <param name="newlen">New value length pointer.</param>
        /// <returns>An error code.</returns>
        /// <remarks>See:
        ///  <see href="https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man3/sysctlbyname.3.html"/>
        /// </remarks>
        [DllImport(Constants.SystemLibrary)]
        private static extern int sysctlbyname(
            [MarshalAs(UnmanagedType.LPStr)] string property,
            byte[] output,
            ref System.IntPtr outputLen,
            byte[] newValue,
            uint newValueLen);
    }
}
