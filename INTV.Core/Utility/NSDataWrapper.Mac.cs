// <copyright file="NSDataWrapper.Mac.cs" company="INTV Funhouse">
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
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using nuint = System.nuint;
#else
using nuint = System.UInt32;
#endif // __UNIFIED__

namespace INTV.Core.Utility
{
    /// <summary>
    /// This class offers a brain-dead wrapper for assisting with NSPasteboard and other
    /// Mac Cocoa APIs requiring NSObject data. It fakes the buffer by returning empty contents.
    /// Fine for intra-process drag and drop, menu items, and other such APIs.
    /// </summary>
    public class NSDataWrapper : NSData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.SingleInstanceApplication"/> class.
        /// </summary>
        /// <param name="handle">Native object handle.</param>
        /// <remarks>Called when created from unmanaged code.
        /// NOTE: Added this in an attempt to address apparently random crashes that are
        /// happening in extremely rare circumstances on application launch.</remarks>
        public NSDataWrapper(System.IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Utility.NSDataWrapper"/> class.
        /// </summary>
        /// <param name="wrappedObject">The standard C# object to wrap inside the NSObject.</param>
        public NSDataWrapper(object wrappedObject)
        {
            WrappedObject = wrappedObject;
        }

        /// <inheritdoc/>
        public override System.IntPtr Bytes
        {
            get { return System.IntPtr.Zero; }
        }

        /// <inheritdoc/>
        public override nuint Length
        {
            get { return 0; }
            set { }
        }

        private object WrappedObject { get; set; }

        /// <summary>
        /// Gets the wrapped object as a specific type.
        /// </summary>
        /// <typeparam name="T">The data type to use when retrieving the wrapped object.</typeparam>
        /// <returns>The wrapped object as the specified type.</returns>
        public T GetWrappedObject<T>()
        {
            return (T)WrappedObject;
        }
    }
}
