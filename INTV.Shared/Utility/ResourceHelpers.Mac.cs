// <copyright file="ResourceHelpers.Mac.cs" company="INTV Funhouse">
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
using System.Text;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using CGSize = CoreGraphics.CGSize;
#else
using CGSize = System.Drawing.SizeF;
#endif // __UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class ResourceHelpers
    {
        /// <summary>
        /// Creates a properly formatted string to locate a resource in an assembly.
        /// </summary>
        /// <param name="type">Any object whose implementation is in the assembly in which the resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the resource within the type's assembly.</param>
        /// <returns>The packed resource string suitable to locate the resource.</returns>
        public static string CreatePackedResourceString(this System.Type type, string relativeResourcePath)
        {
            var resourceString = relativeResourcePath;
            return resourceString;
        }

        #region Image Resource Helpers

        /// <summary>
        /// Loads an image resource from an assembly.
        /// </summary>
        /// <param name="type">Any object whose implementation is in the assembly in which the image resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the image resource within the type's assembly.</param>
        /// <returns>The image resource, or <c>null</c> if not found.</returns>
        public static NSImage LoadImageResource(this System.Type type, string relativeResourcePath)
        {
            var resourceName = type.CreatePackedResourceString(relativeResourcePath);
            var assembly = type.Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);
            var image = InitImageFromStream(stream);
            return image;
        }

        /// <summary>
        /// Loads an image from disk.
        /// </summary>
        /// <param name="imagePath">Absolute path to an image to load from disk.</param>
        /// <returns>The image, or <c>null</c> if not found.</returns>
        public static NSImage LoadImageFromDisk(this string imagePath)
        {
            NSImage image = null;
            if (System.IO.File.Exists(imagePath))
            {
                using (var stream = FileUtilities.OpenFileStream(imagePath))
                {
                    image = InitImageFromStream(stream);
                }
            }
            return image;
        }

        private static NSImage InitImageFromStream(System.IO.Stream stream)
        {
            NSImage image = null;
            var data = NSData.FromStream(stream);
            // Stupid Mac won't let us create images from non-UI thread.
            if (OSDispatcher.IsMainThread)
            {
                image = InitImage(data);
            }
            else
            {
                NSThread.Current.InvokeOnMainThread(() => image = InitImage(data));
            }
            return image;
        }

        private static NSImage InitImage(NSData data)
        {
            var image = new NSImage(data);
            if ((image != null) && !image.IsValid)
            {
                image = null;
            }
            else
            {
                var representations = image.Representations();
                if ((representations != null) && (representations.Length > 0))
                {
                    var representation = representations[0];
                    var verticalScale = representation.Size.Height / representation.PixelsHigh;
                    var horizontalScale = representation.Size.Width / representation.PixelsWide;
                    // On Mac, assumption is 72 DPI. If the scales computed here are <> 1, that means we're
                    // loading something at a different DPI, so let's "scale" the NSImage. Ugh.
                    if ((verticalScale < 1) || (horizontalScale < 1) || (verticalScale > 1) || (horizontalScale > 1))
                    {
                        var scaledHeight = (float)representation.PixelsHigh;
                        var scaledWidth = (float)representation.PixelsWide;
                        image.Size = new CGSize(scaledWidth, scaledHeight);
                    }
                }
            }
            return image;
        }

        #endregion // Image Resource Helpers

        #region PList Helpers

        /// <summary>
        /// Gets a value stored in a NSBundle's info.pplist (InfoDictionary).
        /// </summary>
        /// <param name="bundle">The bundle containing the info.plist.</param>
        /// <param name="infoPListKey">The key identifying the info.plist value to get.</param>
        /// <returns>The value stored in the info.plist, or <c>null</c> if not found, or an error occurs.</returns>
        public static NSObject GetPListValue(this NSBundle bundle, string infoPlistKey)
        {
            NSObject value = null;
            try
            {
                var info = bundle.InfoDictionary;
                info.TryGetValue(new NSString(infoPlistKey), out value);
            }
            catch (System.Exception)
            {
            }
            return value;
        }

        /// <summary>
        /// Sets a value stored in a NSBundle's info.plist (InfoDictionary).
        /// </summary>
        /// <param name="bundle">The bundle containing the info.plist.</param>
        /// <param name="infoPListKey">The key identifying the info.plist value to set.</param>
        /// <param name="value">The new value to assign for the given key.</param>
        /// <returns><c>true</c>, if thet value was set, <c>false</c> otherwise.</returns>
        public static bool SetPListValue(this NSBundle bundle, string infoPListKey, NSObject value)
        {
            var setValue = false;
            try
            {
                var currentValue = bundle.GetPListValue(infoPListKey);
                if ((currentValue != null) && (value != null) && !currentValue.Equals(value))
                {
                    var info = bundle.InfoDictionary;
                    info.SetValueForKey(value, new NSString(infoPListKey));
                    setValue = true;
                }
            }
            catch (System.Exception)
            {
            }
            return setValue;
        }

        /// <summary>
        /// Gets the value of an environment value defined in a plist.
        /// </summary>
        /// <param name="bundle">The bundle whose plist data is to be queried for a value.</param>
        /// <param name="name">Name of the environment variable defined in the plist's LSEnvironment dictionary.</param>
        /// <returns>The environment value.</returns>
        public static object GetEnvironmentValue(this NSBundle bundle, string name)
        {
            object value = null;
            try
            {
                //var info = bundle.InfoDictionary;
                NSObject environmentData = GetPListValue(bundle, "LSEnvironment");
                if (environmentData != null/*info.TryGetValue(new NSString("LSEnvironment"), out environmentData)*/)
                {
                    var environment = environmentData as NSDictionary;
                    NSObject nsObjectValue;
                    if ((environment != null) && environment.TryGetValue((NSString)name, out nsObjectValue))
                    {
                        value = GetEnvironmentValue(nsObjectValue);
                    }
                }
            }
            catch (System.Exception)
            {
                // If anything fails, just return null.
                value = null;
            }
            return value;
        }

        /// <summary>
        /// Gets the value of an environment value defined in a plist.
        /// </summary>
        /// <typeparam name="T">The data type of the expected result.</typeparam>
        /// <param name="bundle">The bundle whose plist data is to be queried for a value.</param>
        /// <param name="name">Name of the environment variable defined in the plist's LSEnvironment dictionary.</param>
        /// <returns>The environment value.</returns>
        public static T GetEnvironmentValue<T>(this NSBundle bundle, string name)
        {
            var value = default(T);
            var rawValue = GetEnvironmentValue(bundle, name);
            if (rawValue != null)
            {
                // Boolean environment variables in plist may show up as char type NSNumbers. However,
                // NSNumber doesn't have a 'char' return value - it uses SByte.
                if (((System.Type.GetTypeCode(typeof(T)) == System.TypeCode.Boolean)) && (System.Type.GetTypeCode(rawValue.GetType()) == System.TypeCode.SByte))
                {
                    rawValue = (sbyte)rawValue != 0;
                }
                value = (T)rawValue;
            }
            return value;
        }

        private static object GetEnvironmentValue(NSObject nsObjectValue)
        {
            object value = null;
            switch (GetTypeCodeFromNSObjectData(nsObjectValue))
            {
                case System.TypeCode.Boolean:
                    value = ((NSNumber)nsObjectValue).BoolValue;
                    break;
                case System.TypeCode.Char:
                    value = ((NSNumber)nsObjectValue).SByteValue;
                    break;
                case System.TypeCode.SByte:
                    value = ((NSNumber)nsObjectValue).SByteValue;
                    break;
                case System.TypeCode.Byte:
                    value = ((NSNumber)nsObjectValue).ByteValue;
                    break;
                case System.TypeCode.Int16:
                    value = ((NSNumber)nsObjectValue).Int16Value;
                    break;
                case System.TypeCode.UInt16:
                    value = ((NSNumber)nsObjectValue).UInt16Value;
                    break;
                case System.TypeCode.Int32:
                    value = ((NSNumber)nsObjectValue).Int32Value;
                    break;
                case System.TypeCode.UInt32:
                    value = ((NSNumber)nsObjectValue).UInt32Value;
                    break;
                case System.TypeCode.Int64:
                    value = ((NSNumber)nsObjectValue).Int64Value;
                    break;
                case System.TypeCode.UInt64:
                    value = ((NSNumber)nsObjectValue).UInt64Value;
                    break;
                case System.TypeCode.Single:
                    value = ((NSNumber)nsObjectValue).FloatValue;
                    break;
                case System.TypeCode.Double:
                    value = ((NSNumber)nsObjectValue).DoubleValue;
                    break;
                case System.TypeCode.Decimal:
                    break;
                case System.TypeCode.DateTime:
                    break;
                case System.TypeCode.String:
                    value = (string)((NSString)nsObjectValue);
                    break;
                default:
                    break;
            }
            return value;
        }

        private static System.TypeCode GetTypeCodeFromNSObjectData(NSObject nsObjectData)
        {
            var typeCode = System.TypeCode.Empty;
            if (nsObjectData != null)
            {
                if (nsObjectData is NSString)
                {
                    typeCode = System.TypeCode.String;
                }
                else
                {
                    var nsValue = nsObjectData as NSValue;
                    if (nsValue != null)
                    {
                        // More than what we really care about here:
                        // https://developer.apple.com/library/ios/documentation/Cocoa/Conceptual/ObjCRuntimeGuide/Articles/ocrtTypeEncodings.html
                        switch (nsValue.ObjCType)
                        {
                            case "c": // A char
                                typeCode = System.TypeCode.Char;
                                break;
                            case "i": // An int
                                typeCode = System.TypeCode.Int32;
                                break;
                            case "s": // A short
                                typeCode = System.TypeCode.Int16;
                                break;
                            case "l": // A long (l is treated as a 32-bit quantity on 64-bit programs.)
                                typeCode = System.TypeCode.Int32;
                                break;
                            case "q": // A long long
                                typeCode = System.TypeCode.Int64;
                                break;
                            case "C": // An unsigned char
                                typeCode = System.TypeCode.Byte;
                                break;
                            case "I": // An unsigned int
                                typeCode = System.TypeCode.UInt32;
                                break;
                            case "S": // An unsigned short
                                typeCode = System.TypeCode.UInt16;
                                break;
                            case "L": // An unsigned long
                                typeCode = System.TypeCode.UInt32;
                                break;
                            case "Q": // An unsigned long long
                                typeCode = System.TypeCode.UInt64;
                                break;
                            case "f": // A float
                                typeCode = System.TypeCode.Single;
                                break;
                            case "d": // A double
                                typeCode = System.TypeCode.Double;
                                break;
                            case "B": // A C++ bool or a C99 _Bool
                                typeCode = System.TypeCode.Boolean;
                                break;
                            case "v": // A void
                                break;
                            case "*": // A character string (char *)
                                typeCode = System.TypeCode.String;
                                break;
                        }
                    }
                }
            }
            return typeCode;
        }

        #endregion // PList Helpers
    }
}
