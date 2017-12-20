// <copyright file="IOIterator.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// A C# class that acts as an analog to the IOKit's IOIterator type.
    /// </summary>
    public class IOIterator : IOKitObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOIterator"/> class.
        /// </summary>
        /// <param name="masterPort">The master port.</param>
        /// <param name="serialServices">The serial services.</param>
        public IOIterator(IOMachPort masterPort, NSMutableDictionary serialServices)
            : base(() => Initialize(masterPort, serialServices))
        {
        }

        internal IOIterator(System.IntPtr iterator)
            : base(iterator)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        public bool IsValid
        {
            get { return NativeMethods.IOIteratorIsValid(Handle) != 0; }
        }

        /// <summary>
        /// Move the iterator to point to the next element in the colleciton.
        /// </summary>
        /// <typeparam name="T">The type of objects being iterated over.</typeparam>
        /// <returns>The iterator to the next element, <c>null</c> if there isn't a next element.</returns>
        public T Next<T>() where T : IOKitObject, new()
        {
            T ioKitObject = null;
            var ioKitObjectHandle = NativeMethods.IOIteratorNext(Handle);
            if (ioKitObjectHandle != System.IntPtr.Zero)
            {
                ioKitObject = IOKitObject.CreateIOKitObject<T>(ioKitObjectHandle);
            }
            return ioKitObject;
        }

        /// <summary>
        /// Reset the iterator.
        /// </summary>
        public void Reset()
        {
            NativeMethods.IOIteratorReset(Handle);
        }

        private static System.IntPtr Initialize(IOMachPort masterPort, NSMutableDictionary serialServices)
        {
            System.IntPtr iterator;
            var result = NativeMethods.IOServicesMatchingDictionary(masterPort.Handle, serialServices, out iterator);
            if (result != NativeMethods.Success)
            {
                throw new System.InvalidOperationException();
            }
            return iterator;
        }
    }
}
