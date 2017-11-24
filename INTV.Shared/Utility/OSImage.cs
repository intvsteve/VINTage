// <copyright file="OSImage.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

#if WIN
using NativeImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using NativeImage = AppKit.NSImage;
#else
using NativeImage = MonoMac.AppKit.NSImage;
using NativeSize = System.Drawing.SizeF;
#endif // __UNIFIED__
#elif GTK
using NativeImage = Gdk.Pixbuf;
#endif // WIN

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Simple wrapper for native Image type.
    /// </summary>
    public partial struct OSImage
    {
        /// <summary>
        /// The canonical empty image.
        /// </summary>
        public static readonly OSImage Empty = new OSImage(null);

        private NativeImage _image;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSImage"/> struct.
        /// </summary>
        /// <param name="image">A platform-specific image.</param>
        public OSImage(NativeImage image)
        {
            _image = image;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _image == null; }
        }

        /// <summary>
        /// Gets the native image.
        /// </summary>
        public NativeImage NativeImage
        {
            get { return _image; }
        }

        /// <summary>Tests two images for equality.</summary>
        /// <param name="lhs">An image to compare.</param>
        /// <param name="rhs">Another image to compare.</param>
        /// <returns><c>true</c> if the images are equal.</returns>
        public static bool operator ==(OSImage lhs, OSImage rhs)
        {
            return object.Equals(lhs._image, rhs._image);
        }

        /// <summary>Tests two images for inequality.</summary>
        /// <param name="lhs">An image to compare.</param>
        /// <param name="rhs">Another image to compare.</param>
        /// <returns><c>true</c> if the images are not equal.</returns>
        public static bool operator !=(OSImage lhs, OSImage rhs)
        {
            return !object.Equals(lhs._image, rhs._image);
        }

        /// <summary>>Wraps a platform-specific image in an abstracted image.</summary>
        /// <param name="image">A platform-specific image to wrap in the abstraction.</param>
        /// <returns>The wrapped image.</returns>
        public static implicit operator OSImage(NativeImage image)
        {
            return new OSImage(image);
        }

        /// <summary>>Unwraps the native image from a platform-abstract image.</summary>
        /// <param name="image">The abstracted image to convert to a platform-specific object.</param>
        /// <returns>The native image.</returns>
        public static implicit operator NativeImage(OSImage image)
        {
            return image._image;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _image == null ? 0 : _image.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is OSImage)
            {
                return object.Equals(_image, ((OSImage)obj)._image);
            }
            else if (obj is NativeImage)
            {
                return object.Equals(_image, (NativeImage)obj);
            }
            return false;
        }
    }
}
