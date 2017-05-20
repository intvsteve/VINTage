// <copyright file="IOMachPort.cs" company="INTV Funhouse">
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

////#define ENABLE_DEBUG_OUTPUT

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// A C# class representing the IOMachPort type from IOKit.
    /// </summary>
    public class IOMachPort : IOKitObject
    {
        /// <summary>
        /// The default bootstrap port.
        /// </summary>
        public static readonly System.IntPtr DefaultBootstrapPort = System.IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Interop.IOKit.IOMachPort"/> class.
        /// </summary>
        public IOMachPort()
            : base(Initialize)
        {
        }

        /// <summary>
        /// Gets an iterator to enumerate RS-232 serial services.
        /// </summary>
        /// <returns>The RS-232 serial services iterator.</returns>
        public IOIterator GetRS232SerialServicesIterator()
        {
            IOIterator iterator = null;
            var servicesDictionary = GetRS232SerialMatchDictionary();
            if (servicesDictionary != null)
            {
                iterator = new IOIterator(this, servicesDictionary);
            }
            return iterator;
        }

        /// <summary>
        /// Gets the RS-232 serial match dictionary.
        /// </summary>
        /// <returns>The RS-232 serial match dictionary.</returns>
        /// <remarks>NOTE: This matching dictionary is specifically configured to locate BSD RS-232 serial ports.</remarks>
        public static NSMutableDictionary GetRS232SerialMatchDictionary()
        {
            var servicesDictionary = IOService.MatchingServicesFromName(NativeMethods.kIOSerialBSDServiceValue);
            if (servicesDictionary != null)
            {
                servicesDictionary.SetValueForKey((NSString)NativeMethods.kIOSerialBSDTypeKey, (NSString)NativeMethods.kIOSerialBSDRS232Type);
            }
            return servicesDictionary;
        }

        /// <summary>
        /// Gets the USB device services iterator.
        /// </summary>
        /// <param name="vendorId">The numeric value of the vendor ID to match. If zero, this is ignored.</param>
        /// <param name="productId">The numeric value of the product ID to match. If zero, this is ignored.</param>
        /// <returns>The USB device services iterator.</returns>
        public IOIterator GetUSBServicesIterator(int vendorId, int productId)
        {
            IOIterator iterator = null;
            var servicesDictionary = GetUSBMatchDictionary(vendorId, productId);
            if (servicesDictionary != null)
            {
                iterator = new IOIterator(this, servicesDictionary);
            }
            return iterator;
        }

        /// <summary>
        /// Gets the USB match dictionary.
        /// </summary>
        /// <param name="vendorId">The numeric value of the vendor ID to match. If zero, this is ignored.</param>
        /// <param name="productId">The numeric value of the product ID to match. If zero, this is ignored.</param>
        /// <returns>The USB match dictionary.</returns>
        /// <remarks>NOTE: This dictionary is configured to match USB Devices -- not USB Interfaces.</remarks>
        public static NSMutableDictionary GetUSBMatchDictionary(int vendorId, int productId)
        {
            var servicesDictionary = IOService.MatchingServicesFromName(NativeMethods.kIOUSBDeviceClassName);
            if (servicesDictionary != null)
            {
                if (vendorId != 0)
                {
                    var vid = NSNumber.FromInt32(vendorId);
                    servicesDictionary.SetValueForKey(vid, (NSString)NativeMethods.kUSBVendorID);
                }
                if (productId != 0)
                {
                    var pid = NSNumber.FromInt32(productId);
                    servicesDictionary.SetValueForKey(pid, (NSString)NativeMethods.kUSBProductID);
                }
            }
            return servicesDictionary;
        }

        private static System.IntPtr Initialize()
        {
            System.IntPtr masterPort = System.IntPtr.Zero;
#if !USE_NULL_MASTER_PORT
            var result = NativeMethods.IOMasterPort(DefaultBootstrapPort, out masterPort);
            if (result != NativeMethods.Success)
            {
                throw new System.InvalidOperationException("Failed to open master port");
            }
#endif
            return masterPort;
        }
    }
}
