// <copyright file="IORegistryEntry.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
#endif

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Class to act as a C# version of the IOKit IORegistryEntry type.
    /// </summary>
    public class IORegistryEntry : IOKitObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IORegistryEntry"/> class.
        /// </summary>
        public IORegistryEntry()
            : base(System.IntPtr.Zero)
        {
        }

        /// <summary>
        /// Gets a property with the given key.
        /// </summary>
        /// <param name="key">The key (name) of the property.</param>
        /// <returns>The property.</returns>
        public NSObject GetProperty(string key)
        {
            var cfProperty = NativeMethods.IORegistryEntryCreateCFProperty(Handle, ((NSString)key).Handle, System.IntPtr.Zero, 0);
            NSObject property = null;
            if (cfProperty != System.IntPtr.Zero)
            {
                property = MonoMac.ObjCRuntime.Runtime.GetNSObject(cfProperty);
            }
            return property;
        }

        /// <summary>
        /// Gets the property as a specific type.
        /// </summary>
        /// <typeparam name="T">The data type of the property, which must be NSObject-based.</typeparam>
        /// <param name="key">The key (name) of the property.</param>
        /// <returns>The property.</returns>
        public T GetProperty<T>(string key) where T : NSObject
        {
            var result = GetProperty(key) as T;
            return result;
        }
    }
}
