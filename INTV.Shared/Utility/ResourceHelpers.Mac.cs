// <copyright file="ResourceHelpers.Mac.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMac.AppKit;

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
        public static string CreatePackedResourceString(this Type type, string relativeResourcePath)
        {
            var resourceString = relativeResourcePath;
            return resourceString;
        }

        /// <summary>
        /// Loads an image resource from an assembly.
        /// </summary>
        /// <param name="type">Any object whose implementation is in the assembly in which the image resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the image resource within the type's assembly.</param>
        /// <returns>The image resource, or <c>null</c> if not found.</returns>
        public static NSImage LoadImageResource(this Type type, string relativeResourcePath)
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
                using (var stream = System.IO.File.OpenRead(imagePath))
                {
                    image = InitImageFromStream(stream);
                }
            }
            return image;
        }

        private static NSImage InitImageFromStream(System.IO.Stream stream)
        {
            NSImage image = null;
            var data = MonoMac.Foundation.NSData.FromStream(stream);
            // Stupid Mac won't let us create images from non-UI thread.
            if (OSDispatcher.IsMainThread)
            {
                image = InitImage(data);
            }
            else
            {
                MonoMac.Foundation.NSThread.Current.InvokeOnMainThread(() => image = InitImage(data));
            }
            return image;
        }

        private static NSImage InitImage(MonoMac.Foundation.NSData data)
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
                        image.Size = new System.Drawing.SizeF(scaledWidth, scaledHeight);
                    }
                }
            }
            return image;
        }
    }
}
